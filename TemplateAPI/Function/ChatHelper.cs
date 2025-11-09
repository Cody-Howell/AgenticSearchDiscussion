using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using QuickType;

namespace TemplateAPI.Function {
    public static class ChatHelper {
        public static async Task<HttpResponseMessage> SendMessageToAIAsync(string message, string aiToken, string serverUrl, IHttpClientFactory httpClientFactory) {
            // Build payload
            var payload = new AiRequest {
                Model = "gpt-oss-120b",
                Messages = new[]
                    {
                        new UserMessage { Role = "user", Content = message }
                    },
                Tools = [
                    new Tool
                    {
                        Type = "function",
                        Function = new CalledFunction
                        {
                            Name = "search_tool",
                            Description = "A tool to search the web for relevant information.",
                            Parameters = new Parameters
                            {
                                Type = "object",
                                Properties = new Dictionary<string, ParameterDescription>
                                {
                                    ["summary"] = new ParameterDescription
                                    {
                                        Type = "string",
                                        Description = "The search query provided by the user."
                                    },
                                    ["maxResults"] = new ParameterDescription
                                    {
                                        Type = "integer",
                                        Description = "The maximum number of results to return."
                                    }
                                },
                                ParametersRequired = new[] { "summary" }
                            }
                        }
                    }
                ]
            };

            var json = JsonConvert.SerializeObject(payload);

            Console.WriteLine("Payload JSON: " + json);

            var client = httpClientFactory.CreateClient();
            var req = new HttpRequestMessage(HttpMethod.Post, serverUrl);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", aiToken);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return await client.SendAsync(req);
        }
    }
}