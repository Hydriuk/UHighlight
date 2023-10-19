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

            foreach (var zone in _testedZones.Values)
            {
                zone.Dispose();
            }
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
            zone.PlayerEntered += (sender, player) => ChatManager.serverSendMessage(
                $"Player {player.GetSteamPlayer().playerID.playerName} entered zone {((HighlightedZone)sender).Category}/{((HighlightedZone)sender).Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );

            zone.PlayerExited += (sender, player) => ChatManager.serverSendMessage(
                $"Player {player.GetSteamPlayer().playerID.playerName} exited zone {((HighlightedZone)sender).Category}/{((HighlightedZone)sender).Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );

            zone.VehicleEntered += (sender, vehicle) => ChatManager.serverSendMessage(
                $"Vehicle {vehicle.asset.FriendlyName} entered zone {((HighlightedZone)sender).Category}/{((HighlightedZone)sender).Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );

            zone.VehicleExited += (sender, vehicle) => ChatManager.serverSendMessage(
                $"Vehicle {vehicle.asset.FriendlyName} exited zone {((HighlightedZone)sender).Category}/{((HighlightedZone)sender).Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );

            zone.ZombieEntered += (sender, zombie) => ChatManager.serverSendMessage(
                $"Zombie {zombie.GetInstanceID()} entered zone {((HighlightedZone)sender).Category}/{((HighlightedZone)sender).Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );

            zone.ZombieExited += (sender, zombie) => ChatManager.serverSendMessage(
                $"Zombie {zombie.GetInstanceID()} exited zone {((HighlightedZone)sender).Category}/{((HighlightedZone)sender).Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );

            zone.AnimalEntered += (sender, animal) => ChatManager.serverSendMessage(
                $"Animal {animal.asset.FriendlyName} entered zone {((HighlightedZone)sender).Category}/{((HighlightedZone)sender).Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );

            zone.AnimalExited += (sender, animal) => ChatManager.serverSendMessage(
                $"Animal {animal.asset.FriendlyName} exited zone {((HighlightedZone)sender).Category}/{((HighlightedZone)sender).Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );

            _effectBuilder.DisplayEffect(zone.Volume);
        }
    }
}