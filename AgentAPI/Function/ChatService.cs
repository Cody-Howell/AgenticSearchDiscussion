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
    private const int MaxTodoBreakdownIterations = 20;

    public async Task<AiResponse> SendMessageAsync(UserMessage[] messages, Tool[]? functions = null) {
        var payload = new AiRequest {
            Model = config.Model,
            Messages = messages,
            Tools = functions
        };

        foreach (var (i, msg) in messages.Select((m, idx) => (idx, m))) {
            Console.WriteLine($"  [{i}] Role={msg.Role}, Content={msg.Content?.Substring(0, Math.Min(50, msg.Content?.Length ?? 0))}, ToolCalls={msg.ToolCalls?.Length}");
        }

        var json = JsonConvert.SerializeObject(payload, QuickType.Converter.Settings);

        var client = httpFactory.CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Post, config.ServerUrl);
        if (!string.IsNullOrWhiteSpace(config.AiToken)) {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.AiToken);
        }
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var resp = await client.SendAsync(req);
        var respText = await resp.Content.ReadAsStringAsync();

        Console.WriteLine($"[ChatService.SendMessageAsync] Request payload: {json}");
        Console.WriteLine($"[ChatService.SendMessageAsync] Response status: {(int)resp.StatusCode}");

        if (!resp.IsSuccessStatusCode) {
            throw new HttpRequestException($"AI server returned {(int)resp.StatusCode}: {respText}");
        }

        try {
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
        Tool[] toolDefinitions,
        Func<string, Task> addTodoItemCallback) {
        
        AiResponse aiResp;
        int count = 0;
        
        while (true) {
            Console.WriteLine("\n\n\n------Starting New Iteration--------");
            if (count < MaxTodoBreakdownIterations) {
                aiResp = await SendMessageAsync([.. messages], toolDefinitions);
            } else {
                aiResp = await SendMessageAsync([.. messages], [toolDefinitions[1]]);
            }

            if (aiResp?.Choices == null || aiResp.Choices.Length == 0) break;
            if (aiResp.Choices[0].FinishReason != "tool_calls") break;

            var toolCall = aiResp.Choices[0].ToolCalls?.FirstOrDefault();
            if (toolCall == null || toolCall.Function == null) break;

            var argsJson = toolCall.Function.Arguments ?? string.Empty;
            string? itemArg = null;

            if (toolCall.Function.Name == "user_take_a_look") {
                messages.Add(new UserMessage {
                    Role = "assistant",
                    ToolCalls = new[] {
                        new UserToolCall {
                            Type = "function",
                            Function = new UserToolFunction {
                                Name = "user_take_a_look",
                                Arguments = argsJson
                            },
                            Id = toolCall.Id
                        }
                    }
                });

                messages.Add(new UserMessage {
                    Role = "tool",
                    ToolCallId = toolCall.Id,
                    Content = JsonConvert.SerializeObject(new { status = "success", message = "User notified to review completed work." })
                });
                break;
            }

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
                    count++;
                    continue;
                }
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
                messages.Add(new UserMessage { Role = "assistant", Content = argsJson });
                break;
            }

            await addTodoItemCallback(itemArg);

            messages.Add(new UserMessage {
                Role = "assistant",
                ToolCalls = new[] {
                    new UserToolCall {
                        Type = "function",
                        Function = new UserToolFunction {
                            Name = toolCall.Function.Name,
                            Arguments = argsJson
                        },
                        Id = toolCall.Id
                    }
                }
            });

            messages.Add(new UserMessage {
                Role = "tool",
                ToolCallId = toolCall.Id,
                Content = JsonConvert.SerializeObject(new { status = "success", message = "Item added successfully.", item = itemArg })
            });
            count++;
        }
        
        return aiResp;
    }

    public async Task<AiResponse> ProcessChatWithFileToolsAsync(
        List<UserMessage> messages,
        Tool[] fileTools,
        Func<string, string, Task<string>> getFileContentsCallback,
        Func<string, Task<string[]>> getFilesInFolderCallback,
        Func<Task<string[]>> getTopLevelFoldersCallback,
        Func<UserMessage, Task>? onProgress = null) {
        
        AiResponse aiResp;
        
        while (true) {
            Console.WriteLine("\n\n\n------Starting Chat Iteration--------");
            aiResp = await SendMessageAsync([.. messages], fileTools);

            if (aiResp?.Choices == null || aiResp.Choices.Length == 0) break;
            if (aiResp.Choices[0].FinishReason != "tool_calls") break;

            var toolCall = aiResp.Choices[0].ToolCalls?.FirstOrDefault();
            if (toolCall == null || toolCall.Function == null) break;

            var argsJson = toolCall.Function.Arguments ?? string.Empty;
            var functionName = toolCall.Function.Name;

            string resultContent;
            
            try {
                switch (functionName) {
                    case "get_top_level_folders": {
                        var folders = await getTopLevelFoldersCallback();
                        resultContent = JsonConvert.SerializeObject(new { status = "success", folders });
                        break;
                    }
                    case "get_files_in_folder": {
                        string? folder = null;
                        try {
                            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(argsJson);
                            if (dict != null && dict.TryGetValue("folder", out var v)) folder = v;
                        } catch { }

                        if (folder == null) {
                            try {
                                var jo = Newtonsoft.Json.Linq.JObject.Parse(argsJson);
                                folder = jo["folder"]?.ToString();
                            } catch { }
                        }

                        if (string.IsNullOrWhiteSpace(folder)) {
                            resultContent = JsonConvert.SerializeObject(new { status = "error", message = "folder parameter is required" });
                        } else {
                            var files = await getFilesInFolderCallback(folder);
                            resultContent = JsonConvert.SerializeObject(new { status = "success", folder, files });
                        }
                        break;
                    }
                    case "get_file_contents": {
                        string? folder = null;
                        string? relpath = null;
                        try {
                            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(argsJson);
                            if (dict != null) {
                                dict.TryGetValue("folder", out folder);
                                dict.TryGetValue("relpath", out relpath);
                            }
                        } catch { }

                        if (folder == null || relpath == null) {
                            try {
                                var jo = Newtonsoft.Json.Linq.JObject.Parse(argsJson);
                                folder = jo["folder"]?.ToString();
                                relpath = jo["relpath"]?.ToString();
                            } catch { }
                        }

                        if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(relpath)) {
                            resultContent = JsonConvert.SerializeObject(new { status = "error", message = "folder and relpath parameters are required" });
                        } else {
                            var contents = await getFileContentsCallback(folder, relpath);
                            resultContent = JsonConvert.SerializeObject(new { status = "success", folder, relpath, contents });
                        }
                        break;
                    }
                    default:
                        resultContent = JsonConvert.SerializeObject(new { status = "error", message = $"Unknown function: {functionName}" });
                        break;
                }
            } catch (Exception ex) {
                resultContent = JsonConvert.SerializeObject(new { status = "error", message = ex.Message });
            }

            var assistantToolCall = new UserMessage {
                Role = "assistant",
                ToolCalls = new[] {
                    new UserToolCall {
                        Type = "function",
                        Function = new UserToolFunction {
                            Name = functionName,
                            Arguments = argsJson
                        },
                        Id = toolCall.Id
                    }
                }
            };
            messages.Add(assistantToolCall);
            if (onProgress != null) await onProgress(assistantToolCall);

            var toolResult = new UserMessage {
                Role = "tool",
                ToolCallId = toolCall.Id,
                Content = resultContent
            };
            messages.Add(toolResult);
            if (onProgress != null) await onProgress(toolResult);
            
            foreach (var item in messages) {
                Console.WriteLine($"Role: {item.Role}, Content: {item.Content}");
            }
        }
        
        return aiResp;
    }
}