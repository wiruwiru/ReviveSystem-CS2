using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using System;

namespace ReviveSystem;

public partial class ReviveSystemBase
{
    [GameEventHandler]
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (player?.UserId == null || !player.IsValid || player.IsHLTV || player.Connected != PlayerConnectedState.PlayerConnected)
        {
            return HookResult.Continue;
        }

        if (!PlayersInfo.ContainsKey(player.UserId.Value))
        {
            PlayersInfo[player.UserId.Value] = new PlayerInfo(player.UserId.Value, player.PlayerName);
        }

        var playerPosition = player.PlayerPawn.Value?.AbsOrigin;
        var playerRotation = player.PlayerPawn.Value?.AbsRotation;

        PlayersInfo[player.UserId.Value].DiePosition = new DiePosition(
            new Vector(
                playerPosition?.X ?? 0,
                playerPosition?.Y ?? 0,
                playerPosition?.Z ?? 0
            ),
            new QAngle(
                playerRotation?.X ?? 0,
                playerRotation?.Y ?? 0,
                playerRotation?.Z ?? 0
            )
        );

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        foreach (var player in PlayersInfo.Values)
        {
            player.DiePosition = null;
        }

        return HookResult.Continue;
    }
}