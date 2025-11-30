using System.Data;
using Dapper;
using TemplateAPI.Classes;

namespace TemplateAPI;

public class DBService(IDbConnection conn) {
    // Create a new chat and return its ID
    public async Task<int> CreateChatAsync() {
        const string sql = @"
            INSERT INTO Chats DEFAULT VALUES 
            RETURNING Id";
        return await conn.ExecuteScalarAsync<int>(sql);
    }
    
    // Get all messages for a chat
    public async Task<IEnumerable<DBMessage>> GetMessagesAsync(int chatId) {
        const string sql = @"
            SELECT Id, ChatId, ChatType as Type, ChatRole as Role, MessageText 
            FROM Messages 
            WHERE ChatId = @ChatId
            ORDER BY Id";
        return await conn.QueryAsync<DBMessage>(sql, new { ChatId = chatId });
    }
    
    // Add a new message
    public async Task AddMessageAsync(DBMessage message) {
        const string sql = @"
            INSERT INTO Messages (ChatId, ChatType, ChatRole, MessageText) 
            VALUES (@ChatId, @Type, @Role, @MessageText)";
        await conn.ExecuteAsync(sql, message);
    }
    
    // Delete a message
    public async Task<bool> DeleteMessageAsync(int id, int chatId) {
        const string sql = "DELETE FROM Messages WHERE Id = @Id AND ChatId = @ChatId";
        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id, ChatId = chatId });
        return rowsAffected > 0;
    }
}