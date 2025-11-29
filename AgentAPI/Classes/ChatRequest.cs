using QuickType;

namespace TemplateAPI.Endpoints {
    public class ChatRequest {
        public UserMessage[] Messages { get; set; } = new UserMessage[0];
        public Tool[]? Functions { get; set; }
    }
}