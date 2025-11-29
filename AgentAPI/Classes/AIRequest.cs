namespace QuickType;

using Newtonsoft.Json;

public partial class AiRequest {
    [JsonProperty("model")]
    public required string Model { get; set; }

    [JsonProperty("messages")]
    public required UserMessage[] Messages { get; set; }

    [JsonProperty("tools")]
    public Tool[]? Tools { get; set; }

    [JsonProperty("tool_choice")]
    public string ToolChoice { get; set; } = "auto";
}

public partial class UserMessage {
    [JsonProperty("role")]
    public required string Role { get; set; }

    [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
    public string? Content { get; set; }

    [JsonProperty("tool_call_id", NullValueHandling = NullValueHandling.Ignore)]
    public string? ToolCallId { get; set; }

    [JsonProperty("function_call", NullValueHandling = NullValueHandling.Ignore)]
    public object? FunctionCall { get; set; }

    [JsonProperty("tool_calls", NullValueHandling = NullValueHandling.Ignore)]
    public object[]? ToolCalls { get; set; }
}

public partial class Tool {
    [JsonProperty("type")]
    public required string Type { get; set; }

    [JsonProperty("function")]
    public required CalledFunction Function { get; set; }
}

public partial class CalledFunction {
    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("description")]
    public required string Description { get; set; }

    [JsonProperty("parameters")]
    public required Parameters Parameters { get; set; }
}

public partial class Parameters {
    [JsonProperty("type")]
    public required string Type { get; set; }

    [JsonProperty("properties")]
    public required Dictionary<string, ParameterDescription> Properties { get; set; }

    [JsonProperty("required")]
    public required string[] ParametersRequired { get; set; }
}

public partial class ParameterDescription {
    [JsonProperty("type")]
    public required string Type { get; set; }

    [JsonProperty("description")]
    public required string Description { get; set; }

    [JsonProperty("items")]
    public Items? Items { get; set; }
}

public partial class Items {
    [JsonProperty("type")]
    public required string Type { get; set; }
}

public partial class AiRequest {
    public static AiRequest FromJson(string json) => JsonConvert.DeserializeObject<AiRequest>(json, QuickType.Converter.Settings) ?? throw new JsonException("Failed to deserialize AiRequest");
}