using QuickType;
using System.Threading.Tasks;

namespace TemplateAPI.Function {
    public interface IChatService {
        Task<AiResponse> SendMessageAsync(UserMessage[] messages, Tool[]? functions = null);
        Task<AiResponse> ProcessTodoBreakdownAsync(
            List<UserMessage> messages,
            Tool toolDefinition,
            Func<string, Task> addTodoItemCallback);
    }
}