using System.Text.Json.Serialization;

namespace VRChatOscDebugger.OscCore.Models;

public class OscQueryHostInfo
{
    [JsonPropertyName("NAME")]
    public string Name { get; set; }

    [JsonPropertyName("EXTENSIONS")]
    public Extensions Extensions { get; set; }

    [JsonPropertyName("OSC_IP")]
    public string OscIp { get; set; }

    [JsonPropertyName("OSC_PORT")]
    public int OscPort { get; set; }

    [JsonPropertyName("OSC_TRANSPORT")]
    public string OscTransport { get; set; }
}

public class Extensions
{
    [JsonPropertyName("ACCESS")]
    public bool Access { get; set; }

    [JsonPropertyName("CLIPMODE")]
    public bool Clipmode { get; set; }

    [JsonPropertyName("RANGE")]
    public bool Range { get; set; }

    [JsonPropertyName("TYPE")]
    public bool Type { get; set; }

    [JsonPropertyName("VALUE")]
    public bool Value { get; set; }
}
