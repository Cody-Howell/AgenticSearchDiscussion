namespace QuickType;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public partial class AiRequest {
    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("messages")]
    public Message[] Messages { get; set; }

    [JsonProperty("tools")]
    public Tool[] Tools { get; set; }

    [JsonProperty("tool_choice")]
    public string ToolChoice { get; set; }
}

public partial class Tool {
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("function")]
    public Function Function { get; set; }
}

public partial class Function {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("parameters")]
    public Parameters Parameters { get; set; }
}

public partial class Parameters {
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("properties")]
    public Properties Properties { get; set; }

    [JsonProperty("required")]
    public string[] ParametersRequired { get; set; }
}

public partial class Properties {
    [JsonProperty("summary")]
    public Summary Summary { get; set; }

    [JsonProperty("new_sports")]
    public NewSports NewSports { get; set; }
}

public partial class NewSports {
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("items")]
    public Items Items { get; set; }
}

public partial class Items {
    [JsonProperty("type")]
    public string Type { get; set; }
}

public partial class Summary {
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }
}

public partial class AiRequest {
    public static AiRequest FromJson(string json) => JsonConvert.DeserializeObject<AiRequest>(json, QuickType.Converter.Settings);
}
