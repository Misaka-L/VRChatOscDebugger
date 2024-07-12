using System.Text.Json.Serialization;
using VRC.OSCQuery;

namespace VRChatOscDebugger.OscCore.Models;

public class OscQueryNode
{
    [JsonPropertyName("DESCRIPTION")] public string? Description { get; set; }
    [JsonPropertyName("FULL_PATH")] public string FullPath { get; set; } = "/";
    [JsonPropertyName("ACCESS")] public Attributes.AccessValues? Access { get; set; }
    [JsonPropertyName("CONTENTS")] public Dictionary<string, OscQueryNode> Contents { get; set; } = [];
    [JsonPropertyName("TYPE")] public string Type { get; set; }
    [JsonPropertyName("VALUE")] public object[]? Value { get; set; }
}
