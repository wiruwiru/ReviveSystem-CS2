using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;

namespace ReviveSystem;

[MinimumApiVersion(296)]
public class ReviveSystemBase : BasePlugin, IPluginConfig<BaseConfigs>
{
    public override string ModuleName => "ReviveSystem";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "luca.uy";
    public override string ModuleDescription => "Allows players to revive one of their teammates per round.";

    public override void Load(bool hotReload)
    {




    }


    public required BaseConfigs Config { get; set; }

    public void OnConfigParsed(BaseConfigs config)
    {
        Config = config;
    }
}