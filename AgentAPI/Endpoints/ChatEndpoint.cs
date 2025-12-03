using Newtonsoft.Json;
using QuickType;
using System.Text;
using TemplateAPI.Endpoints;
using TemplateAPI.Function;
using TemplateAPI.Classes;
using System.Net;

namespace TemplateAPI.Endpoints;

public static class ChatEndpoint {
    public static WebApplication MapChatEndpoints(this WebApplication app) {
        app.MapGet("/api/chats", async (DBService db) => {
            var chats = await db.GetAllChatsAsync();
            return Results.Ok(chats);
        });
        
        app.MapGet("/api/chats/{chatId:int}/messages", async (int chatId, DBService db) => {
            var messages = await db.GetMessagesAsync(chatId);
            return Results.Ok(messages);
        });
        
        app.MapPost("/api/chats/create", async (DBService db) => {
            var chatId = await db.CreateChatAsync();
            return Results.Created($"/api/chats/{chatId}", new { id = chatId });
        });
        
        app.MapPut("/api/chats/{chatId:int}/title", async (int chatId, HttpContext httpContext, DBService db) => {
            string title;
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8)) {
                title = await reader.ReadToEndAsync();
            }
            
            if (string.IsNullOrWhiteSpace(title)) {
                return Results.BadRequest("Title cannot be empty");
            }
            
            // Remove quotes if the title is JSON-stringified
            title = title.Trim('"');
            
            var updated = await db.UpdateChatTitleAsync(chatId, title);
            return updated ? Results.NoContent() : Results.NotFound();
        });
        
        app.MapPost("/api/chats/{chatId:int}/messages", async (int chatId, HttpContext httpContext, DBService db, IChatService chatService, IWebHostEnvironment env, Services.WebSocketService wsService) => {
            string message;
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8)) {
                message = await reader.ReadToEndAsync();
            }
            
            if (string.IsNullOrWhiteSpace(message)) {
                return Results.BadRequest("Message cannot be empty");
            }
            
            // Remove quotes if the message is JSON-stringified
            message = message.Trim('"');
            
            // Store user message
            DBMessage userMsg = new () {ChatId = chatId, MessageText = message, Role = "user", Type = "message"};
            await db.AddMessageAsync(userMsg);
            
            // Send WebSocket notification for user message
            await wsService.SendMessageAsync(chatId, JsonConvert.SerializeObject(new {
                type = "chat_message",
                content = userMsg
            }));
            
            // Get all previous messages for context
            var previousMessages = await db.GetMessagesAsync(chatId);
            var conversationHistory = new List<UserMessage> {
                new UserMessage { Role = "system", Content = TemplateAPI.Classes.SystemMessages.GenericChat }
            };
            
            foreach (var msg in previousMessages) {
                conversationHistory.Add(new UserMessage { 
                    Role = msg.Role, 
                    Content = msg.MessageText 
                });
            }
            
            // Define file tools
            var fileTools = new[] {
                TodoTools.GetTopLevelFoldersTool(),
                TodoTools.GetFilesInFolderTool(),
                TodoTools.GetFileContentsTool()
            };
            
            // Process chat with AI
            AiResponse aiResp;
            try {
                aiResp = await chatService.ProcessChatWithFileToolsAsync(
                    conversationHistory,
                    fileTools,
                    async (folder, relpath) => {
                        return await Task.Run(() => Services.FileService.GetFileContents(folder, relpath, env));
                    },
                    async (folder) => {
                        return await Task.Run(() => Services.FileService.GetFilesInFolder(folder, env));
                    },
                    async () => {
                        return await Task.Run(() => Services.FileService.GetTopLevelFolders(env));
                    }
                );
            } catch (Exception ex) {
                return Results.InternalServerError($"Error calling AI server: {ex.Message}");
            }
            
            // Store all tool calls and tool responses from the conversation
            // Skip the first message (system) and messages we already have in DB
            int skipCount = previousMessages.Count() + 1;
            for (int i = skipCount; i < conversationHistory.Count; i++) {
                var msg = conversationHistory[i];
                
                if (msg.Role == "assistant" && msg.ToolCalls != null) {
                    // Store function call
                    var toolCallJson = JsonConvert.SerializeObject(msg.ToolCalls);
                    DBMessage toolCallMsg = new () {
                        ChatId = chatId, 
                        MessageText = toolCallJson, 
                        Role = "assistant", 
                        Type = "tool_call"
                    };
                    await db.AddMessageAsync(toolCallMsg);
                    
                    // Send WebSocket notification for tool call
                    await wsService.SendMessageAsync(chatId, JsonConvert.SerializeObject(new {
                        type = "chat_message",
                        content = toolCallMsg
                    }));
                } else if (msg.Role == "tool") {
                    // Store tool response
                    DBMessage toolResultMsg = new () {
                        ChatId = chatId, 
                        MessageText = msg.Content ?? "", 
                        Role = "tool", 
                        Type = "tool_result"
                    };
                    await db.AddMessageAsync(toolResultMsg);
                    
                    // Send WebSocket notification for tool result
                    await wsService.SendMessageAsync(chatId, JsonConvert.SerializeObject(new {
                        type = "chat_message",
                        content = toolResultMsg
                    }));
                }
            }
            
            // Store AI response
            if (aiResp?.Choices != null && aiResp.Choices.Length > 0) {
                var aiMessage = aiResp.Choices[0].Message?.Content ?? "";
                if (!string.IsNullOrWhiteSpace(aiMessage)) {
                    DBMessage aiMsg = new () {ChatId = chatId, MessageText = aiMessage, Role = "assistant", Type = "message"};
                    await db.AddMessageAsync(aiMsg);
                    
                    // Send WebSocket notification for AI message
                    await wsService.SendMessageAsync(chatId, JsonConvert.SerializeObject(new {
                        type = "chat_message",
                        content = aiMsg
                    }));
                }
            }
            
            var respJson = JsonConvert.SerializeObject(aiResp);
            return Results.Ok(respJson);
        });
        
        app.MapDelete("/api/chats/{chatId:int}/messages/{id:int}", async (int chatId, int id, DBService db) => {
            var deleted = await db.DeleteMessageAsync(id, chatId);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}