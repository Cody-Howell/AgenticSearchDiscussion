
namespace QuickType;

using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public partial class FunctionResponse {
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

public partial class ToolCall {
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("function")]
    public FunctionCall Function { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }
}

public partial class FunctionCall {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("arguments")]
    public string Arguments { get; set; }
}

public partial class FunctionResponse {
    public static FunctionResponse FromJson(string json) => JsonConvert.DeserializeObject<FunctionResponse>(json, QuickType.Converter.Settings);
}

