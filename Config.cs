using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace ReviveSystem;

public class BaseConfigs : BasePluginConfig
{
    [JsonPropertyName("flag")]
    public string UseFlag { get; set; } = "@css/vip";
    
}