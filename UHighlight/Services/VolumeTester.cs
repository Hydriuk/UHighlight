#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using Hydriuk.UnturnedModules.Extensions;
using SDG.Unturned;
using System.Collections.Generic;
using UHighlight.API;
using UHighlight.Components;
using UnityEngine;

namespace UHighlight.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    internal class VolumeTester : IVolumeTester
    {
        private readonly Dictionary<Player, HighlightedZone> _testedZones = new Dictionary<Player, HighlightedZone>();

        private readonly IHighlightBuilder _highlightBuilder;
        private readonly IEffectBuilder _effectBuilder;

        public VolumeTester(IHighlightBuilder highlightBuilder, IEffectBuilder effectBuilder)
        {
            _highlightBuilder = highlightBuilder;
            _effectBuilder = effectBuilder;

            Provider.onEnemyDisconnected += OnPlayerDisconnected;
        }

        public void Dispose()
        {
            Provider.onEnemyDisconnected -= OnPlayerDisconnected;
        }

        public void StartTest(Player player, string category, string name)
        {
            StopTest(player);

            HighlightedZone zone = _highlightBuilder.BuildZone(category, name);

            InitZone(player, zone);

            _testedZones.Add(player, zone);
        }

        private void OnPlayerDisconnected(SteamPlayer sPlayer) => StopTest(sPlayer.player);
        public void StopTest(Player player)
        {
            if (!_testedZones.TryGetValue(player, out var zone))
                return;

            zone.Dispose();

            _testedZones.Remove(player);
        }

        private void InitZone(Player player, HighlightedZone zone)
        {
            zone.PlayerEntered += (sender, args) => ChatManager.serverSendMessage(
                $"Player {args.Player.GetSteamPlayer().playerID.playerName} entered zone {args.Category}/{args.Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );

            zone.PlayerExited += (sender, args) => ChatManager.serverSendMessage(
                $"Player {args.Player.GetSteamPlayer().playerID.playerName} exited zone {args.Category}/{args.Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );

            zone.VehicleEntered += (sender, args) => ChatManager.serverSendMessage(
                $"Vehicle {args.Vehicle.asset.FriendlyName} entered zone {args.Category}/{args.Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );

            zone.VehicleExited += (sender, args) => ChatManager.serverSendMessage(
                $"Vehicle {args.Vehicle.asset.FriendlyName} exited zone {args.Category}/{args.Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );

            _effectBuilder.DisplayEffect(zone.Volume);
        }
    }
}