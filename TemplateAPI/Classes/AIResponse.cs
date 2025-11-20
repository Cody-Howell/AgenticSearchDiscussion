namespace QuickType;
#pragma warning disable CS8618
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

public partial class AiResponse {
    [JsonProperty("choices")]
    public Choice[] Choices { get; set; }

    [JsonProperty("created")]
    public long Created { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("system_fingerprint")]
    public string SystemFingerprint { get; set; }

    [JsonProperty("object")]
    public string Object { get; set; }

    [JsonProperty("usage")]
    public Usage Usage { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("timings")]
    public Timings Timings { get; set; }
}

public partial class Choice {
    [JsonProperty("finish_reason")]
    public string FinishReason { get; set; }

    [JsonProperty("index")]
    public long Index { get; set; }

    [JsonProperty("message")]
    public Message Message { get; set; }

    [JsonProperty("tool_calls")]
    public ToolCall[]? ToolCalls { get; set; }
}


public partial class ToolCall {
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("function")]
    public Function Function { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }
}

public partial class Function {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("arguments")]
    public string Arguments { get; set; }
}

public partial class Message {
    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("reasoning_content")]
    public string ReasoningContent { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}

public partial class Timings {
    [JsonProperty("cache_n")]
    public long CacheN { get; set; }

    [JsonProperty("prompt_n")]
    public long PromptN { get; set; }

    [JsonProperty("prompt_ms")]
    public double PromptMs { get; set; }

    [JsonProperty("prompt_per_token_ms")]
    public double PromptPerTokenMs { get; set; }

    [JsonProperty("prompt_per_second")]
    public double PromptPerSecond { get; set; }

    [JsonProperty("predicted_n")]
    public long PredictedN { get; set; }

    [JsonProperty("predicted_ms")]
    public double PredictedMs { get; set; }

    [JsonProperty("predicted_per_token_ms")]
    public double PredictedPerTokenMs { get; set; }

    [JsonProperty("predicted_per_second")]
    public double PredictedPerSecond { get; set; }
}

public partial class Usage {
    [JsonProperty("completion_tokens")]
    public long CompletionTokens { get; set; }

    [JsonProperty("prompt_tokens")]
    public long PromptTokens { get; set; }

    [JsonProperty("total_tokens")]
    public long TotalTokens { get; set; }
}

public partial class AiResponse {
    public static AiResponse FromJson(string json) => JsonConvert.DeserializeObject<AiResponse>(json, QuickType.Converter.Settings);
}

public static class Serialize {
    public static string ToJson(this AiResponse self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
}

internal static class Converter {
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
        {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
    };
}

// namespace QuickType
// {

//     using System.Globalization;
//     using Newtonsoft.Json;
//     using Newtonsoft.Json.Converters;

//     public partial class Ignore
//     {
//         [JsonProperty("choices")]
//         public Choice[] Choices { get; set; }

//         [JsonProperty("created")]
//         public long Created { get; set; }

//         [JsonProperty("model")]
//         public string Model { get; set; }

//         [JsonProperty("system_fingerprint")]
//         public string SystemFingerprint { get; set; }

//         [JsonProperty("object")]
//         public string Object { get; set; }

//         [JsonProperty("usage")]
//         public Usage Usage { get; set; }

//         [JsonProperty("id")]
//         public string Id { get; set; }

//         [JsonProperty("timings")]
//         public Timings Timings { get; set; }
//     }

//     public partial class Choice
//     {
//         [JsonProperty("finish_reason")]
//         public string FinishReason { get; set; }

//         [JsonProperty("index")]
//         public long Index { get; set; }

//         [JsonProperty("message")]
//         public Message Message { get; set; }
//     }

//     public partial class Message
//     {
//         [JsonProperty("role")]
//         public string Role { get; set; }

//         [JsonProperty("reasoning_content")]
//         public string ReasoningContent { get; set; }

//         [JsonProperty("content")]
//         public object Content { get; set; }

//         [JsonProperty("tool_calls")]
//         public ToolCall[] ToolCalls { get; set; }
//     }

//     public partial class ToolCall
//     {
//         [JsonProperty("type")]
//         public string Type { get; set; }

//         [JsonProperty("function")]
//         public Function Function { get; set; }

//         [JsonProperty("id")]
//         public string Id { get; set; }
//     }

//     public partial class Function
//     {
//         [JsonProperty("name")]
//         public string Name { get; set; }

//         [JsonProperty("arguments")]
//         public string Arguments { get; set; }
//     }
// }

