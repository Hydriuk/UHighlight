﻿#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using Hydriuk.UnturnedModules.Extensions;
using SDG.Unturned;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly Dictionary<Player, List<HighlightedZone>> _testedZones = new Dictionary<Player, List<HighlightedZone>>();

        private readonly IHighlightBuilder _highlightBuilder;
        private readonly IEffectBuilder _effectBuilder;
        private readonly IChatAdapter _chatAdapter;

        public VolumeTester(IHighlightBuilder highlightBuilder, IEffectBuilder effectBuilder, IChatAdapter chatAdapter)
        {
            _highlightBuilder = highlightBuilder;
            _effectBuilder = effectBuilder;
            _chatAdapter = chatAdapter;

            Provider.onEnemyDisconnected += OnPlayerDisconnected;
        }

        public void Dispose()
        {
            Provider.onEnemyDisconnected -= OnPlayerDisconnected;

            foreach (var zones in _testedZones.Values)
            {
                foreach (var zone in zones)
                {
                    zone.Dispose();
                }
            }
        }

        public async Task StartTest(Player player, string group, string name)
        {
            HighlightedZone zone = await _highlightBuilder.BuildZone(group, name);

            InitZone(player, zone);

            if(!_testedZones.TryGetValue(player, out var zones))
            {
                zones = new List<HighlightedZone>();

                _testedZones.Add(player, zones);
            }

            zones.Add(zone);
        }

        private void OnPlayerDisconnected(SteamPlayer sPlayer) => StopTest(sPlayer.player);
        public Task StopTest(Player player)
        {
            if (!_testedZones.TryGetValue(player, out var zones))
                return Task.CompletedTask;

            foreach (var zone in zones)
            {
                zone.Dispose();
            }

            _testedZones.Remove(player);

            return Task.CompletedTask;
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
            _chatAdapter.Send(player, $"{text} zone {zone.Group}/{zone.Name}");
        }
    }
}