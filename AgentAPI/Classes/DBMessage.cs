namespace TemplateAPI.Classes;

public class DBMessage {
    public int Id { get; set; }= -1;
    public int ChatId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
}
