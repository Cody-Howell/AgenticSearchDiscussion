using Newtonsoft.Json;
using QuickType;
using System.Net.Http.Headers;
using System.Text;

namespace TemplateAPI.Function;

public class ChatServiceConfig {
    public string ServerUrl { get; set; } = string.Empty;
    public string AiToken { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-oss-120b";
}

public class ChatService(IHttpClientFactory httpFactory, ChatServiceConfig config) : IChatService {
    private const int MaxTodoBreakdownIterations = 8;

    public async Task<AiResponse> SendMessageAsync(UserMessage[] messages, Tool[]? functions = null) {
        var payload = new AiRequest {
            Model = config.Model,
            Messages = messages,
            Tools = functions
        };

        var json = JsonConvert.SerializeObject(payload, QuickType.Converter.Settings);

        var client = httpFactory.CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Post, config.ServerUrl);
        if (!string.IsNullOrWhiteSpace(config.AiToken)) {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.AiToken);
        }
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var resp = await client.SendAsync(req);
        var respText = await resp.Content.ReadAsStringAsync();

        // Console.WriteLine(respText);

        if (!resp.IsSuccessStatusCode) {
            throw new HttpRequestException($"AI server returned {(int)resp.StatusCode}: {respText}");
        }

        try {
            // Normalize possible placements/names of function/tool call metadata so our typed model can parse it.
            var j = Newtonsoft.Json.Linq.JObject.Parse(respText);

            var choices = j["choices"] as Newtonsoft.Json.Linq.JArray;
            if (choices != null) {
                foreach (var choice in choices) {
                    var toolCalls = choice["tool_calls"];
                    if (toolCalls == null || toolCalls.Type == Newtonsoft.Json.Linq.JTokenType.Null) {
                        var message = choice["message"] as Newtonsoft.Json.Linq.JObject;
                        if (message != null) {
                            var candidate = message["tool_calls"] ?? message["tool_call"] ?? message["function_call"] ?? message["function_calls"];
                            if (candidate != null && candidate.Type != Newtonsoft.Json.Linq.JTokenType.Null) {
                                if (candidate.Type == Newtonsoft.Json.Linq.JTokenType.Object) {
                                    var arr = new Newtonsoft.Json.Linq.JArray {
                                        candidate
                                    };
                                    choice["tool_calls"] = arr;
                                } else {
                                    choice["tool_calls"] = candidate;
                                }
                            }
                        }
                    }
                }
            }

            var normalized = j.ToString();
            var aiResp = JsonConvert.DeserializeObject<AiResponse>(normalized);
            if (aiResp == null) {
                throw new JsonSerializationException("AI server returned empty or invalid JSON response.");
            }
            return aiResp;
        } catch (JsonException ex) {
            throw new InvalidOperationException("Failed to parse AI server response.", ex);
        }
    }

    public async Task<AiResponse> ProcessTodoBreakdownAsync(
        List<UserMessage> messages,
        Tool toolDefinition,
        Func<string, Task> addTodoItemCallback) {
        
        AiResponse aiResp;
        int count = 0;
        
        while (true) {
            Console.WriteLine("\n\n\n------Starting New Iteration--------");
            if (count < MaxTodoBreakdownIterations) {
                aiResp = await SendMessageAsync([.. messages], [toolDefinition]);
            } else {
                aiResp = await SendMessageAsync([.. messages], []);
            }

            if (aiResp?.Choices == null || aiResp.Choices.Length == 0) break;
            if (aiResp.Choices[0].FinishReason != "tool_calls") break;

            var toolCall = aiResp.Choices[0].ToolCalls?.FirstOrDefault();
            if (toolCall == null || toolCall.Function == null) break;

            var argsJson = toolCall.Function.Arguments ?? string.Empty;
            string? itemArg = null;

            // If the tool call is an assistant instruction, add it as an assistant message
            if (toolCall.Function.Name == "assistant_instruction") {
                string? instruction = null;
                try {
                    var dictInst = JsonConvert.DeserializeObject<Dictionary<string, string>>(argsJson);
                    if (dictInst != null && dictInst.TryGetValue("instruction", out var v)) instruction = v;
                } catch { }

                if (instruction == null) {
                    try {
                        var joInst = Newtonsoft.Json.Linq.JObject.Parse(argsJson);
                        instruction = joInst["instruction"]?.ToString();
                    } catch { }
                }

                if (!string.IsNullOrWhiteSpace(instruction)) {
                    messages.Add(new UserMessage { Role = "assistant", Content = instruction });
                    // continue to next iteration so instruction can influence subsequent calls
                    count++;
                    continue;
                }
                // if we couldn't extract an instruction, fall through and treat as raw assistant content
            }
            
            try {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(argsJson);
                if (dict != null && dict.TryGetValue("item", out var v)) itemArg = v;
            } catch { }

            if (itemArg == null) {
                try {
                    var jo = Newtonsoft.Json.Linq.JObject.Parse(argsJson);
                    itemArg = jo["item"]?.ToString();
                } catch { }
            }

            if (string.IsNullOrWhiteSpace(itemArg)) {
                // If we couldn't extract an item, add the raw args as an assistant message and stop looping
                messages.Add(new UserMessage { Role = "assistant", Content = argsJson });
                break;
            }

            // Call the callback to add the item
            await addTodoItemCallback(itemArg);

            // First, append an assistant message representing the function/tool call
            // so the AI server sees the assistant initiated a tool call.
            messages.Add(new UserMessage {
                Role = "assistant",
                ToolCalls = new[] {
                    new {
                        type = "function",
                        function = new {
                            name = toolCall.Function.Name,
                            arguments = argsJson
                        },
                        id = toolCall.Id
                    }
                }
            });

            // Then append the added item as a `tool` message including the tool_call_id
            // Content is JSON so the AI can parse a structured result.
            messages.Add(new UserMessage {
                Role = "tool",
                ToolCallId = toolCall.Id,
                Content = JsonConvert.SerializeObject(new { status = "success", message = "Item added successfully.", item = itemArg })
            });
            
            foreach (var item in messages) {
                Console.WriteLine($"Role: {item.Role}, Content: {item.Content}");
            }
            count++;
        }
        
        return aiResp;
    }
}