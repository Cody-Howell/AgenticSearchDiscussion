using QuickType;

namespace TemplateAPI.Classes;

public static class SystemMessages {
    public static readonly string TodoBreakdown = """
            Take in an input from the user. The first message is the 
            Break down the message into discrete chunks for an AI agent to read from, one at a time sequentially. 
            Call the function until you've parsed the entire message and have no more items. See previous user messages 
            for the result of your tool calls. Don't duplicate items.
            Once done, provide a message to the user saying that you're done. Make it as short as possible. 
            """;
}