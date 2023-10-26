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
            zone.PlayerEntered += (sender, player) => SendMessage(player, (HighlightedZone)sender, $"Player {player.GetSteamPlayer().playerID.playerName} entered");
            zone.PlayerExited += (sender, player) => SendMessage(player, (HighlightedZone)sender, $"Player {player.GetSteamPlayer().playerID.playerName} exited");

            zone.VehicleEntered += (sender, vehicle) => SendMessage(player, (HighlightedZone)sender, $"Vehicle {vehicle.asset.FriendlyName} entered");
            zone.VehicleExited += (sender, vehicle) => SendMessage(player, (HighlightedZone)sender, $"Vehicle {vehicle.asset.FriendlyName} exited");

            zone.ZombieEntered += (sender, zombie) => SendMessage(player, (HighlightedZone)sender, $"Zombie {zombie.GetInstanceID()} entered");
            zone.ZombieExited += (sender, zombie) => SendMessage(player, (HighlightedZone)sender, $"Zombie {zombie.GetInstanceID()} exited");

            zone.AnimalEntered += (sender, animal) => SendMessage(player, (HighlightedZone)sender, $"Animal {animal.asset.FriendlyName} entered");
            zone.AnimalExited += (sender, animal) => SendMessage(player, (HighlightedZone)sender, $"Animal {animal.asset.FriendlyName} exited");

            zone.BarricadeEntered += (sender, barricade) => SendMessage(player, (HighlightedZone)sender, $"Barricade {barricade.asset.FriendlyName} entered");
            zone.BarricadeExited += (sender, barricade) => SendMessage(player, (HighlightedZone)sender, $"Barricade {barricade.asset.FriendlyName} exited");

            zone.StructureEntered += (sender, structure) => SendMessage(player, (HighlightedZone)sender, $"Structure {structure.asset.FriendlyName} entered");
            zone.StructureExited += (sender, structure) => SendMessage(player, (HighlightedZone)sender, $"Structure {structure.asset.FriendlyName} exited");

            _effectBuilder.DisplayEffect(zone.Volume);
        }

        private void SendMessage(Player player, HighlightedZone zone, string text)
        {
            ChatManager.serverSendMessage(
                $"{text} zone {zone.Category}/{zone.Name}",
                Color.gray,
                toPlayer: player.GetSteamPlayer()
            );
        }
    }
}