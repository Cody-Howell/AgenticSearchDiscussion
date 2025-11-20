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
    public async Task<AiResponse> SendMessageAsync(UserMessage[] messages, Tool[]? functions = null) {
        var payload = new AiRequest {
            Model = config.Model,
            Messages = messages,
            Tools = functions
        };

        var json = JsonConvert.SerializeObject(payload);

        var client = httpFactory.CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Post, config.ServerUrl);
        if (!string.IsNullOrWhiteSpace(config.AiToken)) {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.AiToken);
        }
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var resp = await client.SendAsync(req);
        var respText = await resp.Content.ReadAsStringAsync();

        Console.WriteLine(respText);

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
        } catch (Newtonsoft.Json.JsonException ex) {
            throw new InvalidOperationException("Failed to parse AI server response.", ex);
        }
    }
}