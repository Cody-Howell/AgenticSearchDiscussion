namespace QuickType;

using Newtonsoft.Json;

public partial class AiRequest {
    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("messages")]
    public UserMessage[] Messages { get; set; }

    [JsonProperty("tools")]
    public Tool[]? Tools { get; set; }

    [JsonProperty("tool_choice")]
    public string ToolChoice { get; set; } = "auto";
}

public partial class UserMessage {
        [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}

public partial class Tool {
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("function")]
    public CalledFunction Function { get; set; }
}

public partial class CalledFunction {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("parameters")]
    public Parameters Parameters { get; set; }
}

public partial class Parameters
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("properties")]
    public Dictionary<string, ParameterDescription> Properties { get; set; }

    [JsonProperty("required")]
    public string[] ParametersRequired { get; set; }
}

public partial class ParameterDescription
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("items")]
    public Items? Items { get; set; }
}

public partial class Items
{
    [JsonProperty("type")]
    public string Type { get; set; }
}

public partial class AiRequest {
    public static AiRequest FromJson(string json) => JsonConvert.DeserializeObject<AiRequest>(json, QuickType.Converter.Settings);
}
