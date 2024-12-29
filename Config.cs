using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace ReviveSystem;

public class BaseConfigs : BasePluginConfig
{
    [JsonPropertyName("PermissionFlag")]
    public string PermissionFlag { get; set; } = "@css/vip";

    [JsonPropertyName("ReviveTime")]
    public int ReviveTime { get; set; } = 15;

    [JsonPropertyName("ReviveRange")]
    public float ReviveRange { get; set; } = 100.0f;

    [JsonPropertyName("MaxRevivesPerRound")]
    public int MaxRevivesPerRound { get; set; } = 1;
}
