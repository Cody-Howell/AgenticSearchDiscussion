using QuickType;
using System.Threading.Tasks;

namespace TemplateAPI.Function {
    public interface IChatService {
        Task<AiResponse> SendMessageAsync(UserMessage[] messages, Tool[]? functions = null);
        Task<AiResponse> ProcessTodoBreakdownAsync(
            List<UserMessage> messages,
            Tool[] toolDefinitions,
            Func<string, Task> addTodoItemCallback);
        Task<AiResponse> ProcessChatWithFileToolsAsync(
            List<UserMessage> messages,
            Tool[] fileTools,
            Func<string, string, Task<string>> getFileContentsCallback,
            Func<string, Task<string[]>> getFilesInFolderCallback,
            Func<Task<string[]>> getTopLevelFoldersCallback,
            Func<UserMessage, Task>? onProgress = null);
    }
}