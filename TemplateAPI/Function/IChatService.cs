using QuickType;
using System.Threading.Tasks;

namespace TemplateAPI.Function {
    public interface IChatService {
        Task<AiResponse> SendMessageAsync(UserMessage[] messages, Tool[]? functions = null);
    }
}