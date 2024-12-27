using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Collections.Generic;

namespace ReviveSystem;

[MinimumApiVersion(296)]
public partial class ReviveSystemBase : BasePlugin, IPluginConfig<BaseConfigs>
{
    public override string ModuleName => "ReviveSystem";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "luca.uy";
    public override string ModuleDescription => "Allows players to revive one of their teammates per round.";

    private static MemoryFunctionVoid<CBasePlayerController, CCSPlayerPawn, bool, bool>? _cBasePlayerControllerSetPawnFunc;
    internal static readonly Dictionary<int, PlayerInfo> PlayersInfo = new();

    public override void Load(bool hotReload)
    {
        _cBasePlayerControllerSetPawnFunc = new MemoryFunctionVoid<CBasePlayerController, CCSPlayerPawn, bool, bool>(
            GameData.GetSignature("CBasePlayerController_SetPawn")
        );

        AddCommand(Config.Command, "", (player, info) =>
        {
            if (player == null)
            {
                return;
            }

            var validador = new RequiresPermissions(Config.UseFlag);
            validador.Command = Config.Command;
            if (!validador.CanExecuteCommand(player))
            {
                player.PrintToChat($"{Localizer["prefix"]} {Localizer["NoPermissions"]}");
                return;
            }

            RespawnPlayer(player, player);
        });
    }

    public void RespawnPlayer(CCSPlayerController caller, CCSPlayerController player)
    {
        if (player == null || player.UserId == null || !PlayersInfo.ContainsKey(player.UserId.Value))
        {
            return;
        }

        var playerInfo = PlayersInfo[player.UserId.Value];

        if (player.PlayerPawn.Value == null || !player.PlayerPawn.IsValid)
        {
            return;
        }

        var playerPawn = player.PlayerPawn.Value;
        _cBasePlayerControllerSetPawnFunc?.Invoke(player, playerPawn, true, false);

        VirtualFunction.CreateVoid<CCSPlayerController>(player.Handle, GameData.GetOffset("CCSPlayerController_Respawn"))(player);

        if (playerInfo.DiePosition.HasValue)
        {
            var diePosition = playerInfo.DiePosition.Value;
            playerPawn.Teleport(diePosition.Position, diePosition.Angle);
        }
        else
        {
            Console.WriteLine($"[ReviveSystem] No stored death position found for #{player.UserId}.");
        }
    }


    public required BaseConfigs Config { get; set; }

    public void OnConfigParsed(BaseConfigs config)
    {
        Config = config;
    }

    public class PlayerInfo
    {
        public int? UserId { get; }
        public string Name { get; }
        public DiePosition? DiePosition { get; set; }

        public PlayerInfo(int? userId, string name)
        {
            UserId = userId;
            Name = name;
            DiePosition = null;
        }
    }

    public struct DiePosition
    {
        public Vector Position { get; set; }
        public QAngle Angle { get; set; }

        public DiePosition(Vector position, QAngle angle)
        {
            Position = position;
            Angle = angle;
        }
    }
}
