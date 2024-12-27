using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace ReviveSystem;

public class BaseConfigs : BasePluginConfig
{
    [JsonPropertyName("Command")]
    public string Command { get; set; } = "css_revive";

    [JsonPropertyName("flag")]
    public string UseFlag { get; set; } = "@css/root";
    
}