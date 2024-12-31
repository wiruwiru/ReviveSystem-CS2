using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using static CounterStrikeSharp.API.Core.Listeners;

namespace ReviveSystem
{
    [MinimumApiVersion(296)]
    public partial class ReviveSystemBase : BasePlugin, IPluginConfig<BaseConfigs>
    {
        public override string ModuleName => "ReviveSystem";
        public override string ModuleVersion => "0.1.3-beta";
        public override string ModuleAuthor => "luca.uy";
        public override string ModuleDescription => "Allows players to revive one of their teammates per round.";

        private static MemoryFunctionVoid<CBasePlayerController, CCSPlayerPawn, bool, bool>? _cBasePlayerControllerSetPawnFunc;
        internal static readonly Dictionary<int, PlayerInfo> PlayersInfo = new();
        private readonly Dictionary<int, DateTime> LastBeaconTimes = new();
        private readonly Dictionary<int, DateTime> LastUpdateTimes = new();

        public required BaseConfigs Config { get; set; }

        public override void Load(bool hotReload)
        {

            _cBasePlayerControllerSetPawnFunc = new MemoryFunctionVoid<CBasePlayerController, CCSPlayerPawn, bool, bool>(
                GameData.GetSignature("CBasePlayerController_SetPawn")
            );

            RegisterListener<OnTick>(OnTick);
        }

        public override void Unload(bool hotReload)
        {
            RemoveListener<OnTick>(OnTick);
        }

        private void OnTick()
        {
            foreach (var player in Utilities.GetPlayers())
            {
                if (!player.IsValid || player.IsBot || !player.UserId.HasValue || !PlayersInfo.ContainsKey(player.UserId.Value))
                {
                    continue;
                }

                if (!player.PawnIsAlive)
                {
                    continue;
                }

                var playerInfo = PlayersInfo[player.UserId.Value];

                if (player.Buttons.HasFlag(PlayerButtons.Use))
                {

                    var targetPlayerEntity = Utilities.GetPlayers().FirstOrDefault(p => PlayersInfo.Values.Any(t => t.UserId == p.UserId && t.DiePosition.HasValue));
                    if (targetPlayerEntity == null || targetPlayerEntity.TeamNum != player.TeamNum)
                    {
                        var currentTime = DateTime.Now;
                        var cooldownTime = TimeSpan.FromSeconds(1);

                        if (!playerInfo.LastReviveLimitMessageTime.HasValue || (currentTime - playerInfo.LastReviveLimitMessageTime.Value) >= cooldownTime)
                        {
                            if (targetPlayerEntity != null)
                            {
                                player.PrintToChat($"{Localizer["prefix"]} {Localizer["DifferentTeam"]}");
                            }

                            playerInfo.LastReviveLimitMessageTime = currentTime;
                        }

                        playerInfo.UseStartTime = null;
                        return;
                    }

                    if (playerInfo.ReviveCount >= Config.MaxRevivesPerRound)
                    {
                        var currentTime = DateTime.Now;
                        var cooldownTime = TimeSpan.FromSeconds(1);
                        if (!playerInfo.LastReviveLimitMessageTime.HasValue || (currentTime - playerInfo.LastReviveLimitMessageTime.Value) >= cooldownTime)
                        {
                            player.PrintToChat($"{Localizer["prefix"]} {Localizer["ReviveLimitReached"]}");
                            playerInfo.LastReviveLimitMessageTime = currentTime;
                        }
                        playerInfo.UseStartTime = null;
                        continue;
                    }

                    if (!playerInfo.UseStartTime.HasValue)
                    {
                        playerInfo.UseStartTime = DateTime.Now;
                    }

                    foreach (var targetPlayer in PlayersInfo.Values)
                    {
                        if (targetPlayer.DiePosition.HasValue &&
                            player.PlayerPawn?.Value != null &&
                            CalculateDistance(player.PlayerPawn.Value.AbsOrigin, targetPlayer.DiePosition.Value.Position) <= Config.ReviveRange)
                        {
                            int playerId = player.UserId.Value;

                            if (!LastBeaconTimes.ContainsKey(playerId) || (DateTime.Now - LastBeaconTimes[playerId]).TotalMilliseconds >= 1000)
                            {
                                DrawBeaconOnPlayer(player);
                                LastBeaconTimes[playerId] = DateTime.Now;
                            }

                            var pressDuration = (DateTime.Now - playerInfo.UseStartTime.Value).TotalSeconds;

                            if (!LastUpdateTimes.ContainsKey(playerId) || (DateTime.Now - LastUpdateTimes[playerId]).TotalMilliseconds >= 40)
                            {
                                var progressBarLength = 20;
                                var filledLength = (int)(progressBarLength * (pressDuration / Config.ReviveTime));
                                var emptyLength = progressBarLength - filledLength;
                                var progressBar = new string('▌', filledLength) + new string('░', emptyLength);
                                var percentage = (int)((pressDuration / Config.ReviveTime) * 100);

                                // player.PrintToCenterHtml($"{Localizer["prefix"]} {string.Format(Localizer["Reviving"], targetPlayer.Name)}: 『{progressBar}』 {percentage}%");
                                player.PrintToCenterHtml($"{string.Format(Localizer["Reviving"], targetPlayer.Name)} 『{progressBar}』 {percentage}%");
                                LastUpdateTimes[playerId] = DateTime.Now;
                            }

                            if (pressDuration >= Config.ReviveTime && CanRevive(player, targetPlayer))
                            {
                                player.PrintToChat($"{Localizer["prefix"]} {string.Format(Localizer["ReviveComplete"], targetPlayer.Name)}");
                                RespawnPlayer(player, targetPlayer);
                                playerInfo.UseStartTime = null;

                                playerInfo.ReviveCount++;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (playerInfo.UseStartTime.HasValue)
                    {
                        bool hasValidTarget = PlayersInfo.Values.Any(targetPlayer =>
                            targetPlayer.DiePosition.HasValue &&
                            player.PlayerPawn?.Value != null &&
                            CalculateDistance(player.PlayerPawn.Value.AbsOrigin, targetPlayer.DiePosition.Value.Position) <= Config.ReviveRange
                        );

                        if (hasValidTarget)
                        {
                            player.PrintToChat($"{Localizer["prefix"]} {Localizer["ReviveCancelled"]}");
                        }

                        playerInfo.UseStartTime = null;
                    }

                }
            }
        }

        private float CalculateDistance(Vector? a, Vector? b)
        {
            if (a == null || b == null)
            {
                return float.MaxValue;
            }

            var distance = MathF.Sqrt(MathF.Pow(a.X - b.X, 2) + MathF.Pow(a.Y - b.Y, 2) + MathF.Pow(a.Z - b.Z, 2));
            return distance;
        }

        private bool CanRevive(CCSPlayerController player, PlayerInfo targetPlayer)
        {
            if (!string.IsNullOrEmpty(Config.PermissionFlag) &&
                !AdminManager.PlayerHasPermissions(player, Config.PermissionFlag))
            {
                player.PrintToChat($"{Localizer["prefix"]} {Localizer["NoPermissions"]}");
                return false;
            }

            if (!player.PawnIsAlive)
            {
                return false;
            }

            return true;
        }

        public void RespawnPlayer(CCSPlayerController caller, PlayerInfo targetPlayerInfo)
        {

            var player = Utilities.GetPlayers().FirstOrDefault(p => p.UserId == targetPlayerInfo.UserId);
            if (player == null || player.PlayerPawn?.Value == null || !player.PlayerPawn.IsValid)
            {
                return;
            }

            var playerPawn = player.PlayerPawn.Value;
            _cBasePlayerControllerSetPawnFunc?.Invoke(player, playerPawn, true, false);

            VirtualFunction.CreateVoid<CCSPlayerController>(player.Handle, GameData.GetOffset("CCSPlayerController_Respawn"))(player);

            if (targetPlayerInfo.DiePosition.HasValue)
            {
                var diePosition = targetPlayerInfo.DiePosition.Value;
                playerPawn.Teleport(diePosition.Position, diePosition.Angle);
            }

            targetPlayerInfo.DiePosition = null;
        }

        public void OnConfigParsed(BaseConfigs config)
        {
            Config = config;
        }

        public class PlayerInfo
        {
            public int? UserId { get; }
            public string Name { get; }
            public DiePosition? DiePosition { get; set; }
            public int ReviveCount { get; set; } = 0;
            public DateTime? UseStartTime { get; set; }
            public DateTime? LastReviveLimitMessageTime { get; set; }

            public PlayerInfo(int? userId, string name)
            {
                UserId = userId;
                Name = name;
                DiePosition = null;
                UseStartTime = null;
                LastReviveLimitMessageTime = null;
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
}
