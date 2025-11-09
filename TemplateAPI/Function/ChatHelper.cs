using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using QuickType;

namespace TemplateAPI.Function
{
    public static class ChatHelper
    {
        public static async Task<HttpResponseMessage> SendMessageToAIAsync(string message, string aiToken, string serverUrl, IHttpClientFactory httpClientFactory)
        {
            // Build payload
            var payload = new AiRequest
            {
                Model = "gpt-oss-120b",
                Messages = new[] { new UserMessage { Role = "user", Content = message } }
            };

            var json = JsonConvert.SerializeObject(payload);

            var client = httpClientFactory.CreateClient();
            var req = new HttpRequestMessage(HttpMethod.Post, serverUrl);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", aiToken);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return await client.SendAsync(req);
        }
    }
}