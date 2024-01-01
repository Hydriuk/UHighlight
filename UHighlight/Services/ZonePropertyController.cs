using Hydriuk.UnturnedModules.Adapters;
using Hydriuk.UnturnedModules.Extensions;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UHighlight.API;
using UHighlight.Components;
using UHighlight.DAL;
using UHighlight.Extensions;
using UHighlight.Models;
using UnityEngine;
using static SDG.Unturned.WeatherAsset;
using Random = UnityEngine.Random;

namespace UHighlight.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    internal class ZonePropertyController : IZonePropertyController
    {
        private readonly List<HighlightedZone> _spawnedZones = new List<HighlightedZone>();
        private readonly List<HighlightedZone> _positionnalZones = new List<HighlightedZone>();

        private readonly Dictionary<string, ZoneGroup> _configuration;

        private readonly ICommandAdapter _commandAdapter;

        public ZonePropertyController(IHighlightSpawner highlightSpawner, ICommandAdapter commandAdapter, IVolumeStore volumeStore)
        {
            _commandAdapter = commandAdapter;

            IEnumerable<ZoneGroup> groups = volumeStore.GetGroups();
            
            foreach (ZoneGroup group in groups)
            {
                IEnumerable<HighlightedZone> zones = highlightSpawner.BuildZones(group.Name);
                _spawnedZones.AddRange(zones);

                if (group.PositionnalProperties.Count > 0)
                {
                    _positionnalZones.AddRange(zones);
                }

                if(group.EventProperties.Count > 0)
                {
                    foreach (var zone in zones)
                    {
                        zone.PlayerEntered += OnPlayerEnteredZone;
                        zone.PlayerExited += OnPlayerExitedZone;
                    }
                }
            }

            _configuration = groups.ToDictionary(group => group.Name);

            StructureManager.onDeployStructureRequested += OnStructureDeploying;
            BarricadeManager.onDeployBarricadeRequested += OnBarricadeDeploying;

            StructureManager.onDamageStructureRequested += OnBuildableDamaging;
            BarricadeManager.onDamageBarricadeRequested += OnBuildableDamaging;
            DamageTool.damagePlayerRequested += OnPlayerDamaging;
            DamageTool.damageZombieRequested += OnZombieDamaging;
            DamageTool.damageAnimalRequested += OnAnimalDamaging;
            VehicleManager.onDamageVehicleRequested += OnVehicleDamaging;
        }

        public void Dispose()
        {
            StructureManager.onDeployStructureRequested -= OnStructureDeploying;
            BarricadeManager.onDeployBarricadeRequested -= OnBarricadeDeploying;

            StructureManager.onDamageStructureRequested -= OnBuildableDamaging;
            BarricadeManager.onDamageBarricadeRequested -= OnBuildableDamaging;
            DamageTool.damagePlayerRequested -= OnPlayerDamaging;
            DamageTool.damageZombieRequested -= OnZombieDamaging;
            DamageTool.damageAnimalRequested -= OnAnimalDamaging;
            VehicleManager.onDamageVehicleRequested -= OnVehicleDamaging;

            foreach (var zone in _spawnedZones)
            {
                zone.Dispose();
            }
        }

        #region PlaceStructure
        private void OnStructureDeploying(Structure structure, ItemStructureAsset asset, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            if (!CanDeployStructure(point))
                shouldAllow = false;
        }

        private void OnBarricadeDeploying(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            if (!CanDeployStructure(point))
                shouldAllow = false;
        }

        private bool CanDeployStructure(Vector3 point)
        {
            return _positionnalZones
                .Where(zone => zone.Collides(point))
                .Select(zone => _configuration[zone.Group].PositionnalProperties)
                .SelectMany(properties => properties.Where(property => property.Type == PositionnalZoneProperty.EType.PlaceStructure))
                .Any();
        }
        #endregion

        #region Damage
        private void OnBuildableDamaging(CSteamID instigatorSteamID, Transform buildableTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            float damageMultiplier = _positionnalZones
                .Where(zone => zone.Collides(buildableTransform.position))
                .Select(zone => _configuration[zone.Group].PositionnalProperties)
                .SelectMany(properties => properties.Where(property => property.Type == PositionnalZoneProperty.EType.StructureDamage))
                .Select(property => float.Parse(property.Data))
                .Aggregate((acc, cur) => acc * cur);

            pendingTotalDamage = ConvertDamage(pendingTotalDamage * damageMultiplier);

            if(pendingTotalDamage == 0)
            {
                shouldAllow = false;
            }
        }

        private void OnPlayerDamaging(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            Vector3 point = parameters.player.transform.position;

            float damageMultiplier = _positionnalZones
                .Where(zone => zone.Collides(point))
                .Select(zone => _configuration[zone.Group].PositionnalProperties)
                .SelectMany(properties => properties.Where(property => property.Type == PositionnalZoneProperty.EType.PlayerDamage))
                .Select(property => float.Parse(property.Data))
                .Aggregate((acc, cur) => acc * cur);

            parameters.damage *= damageMultiplier;

            if (parameters.damage == 0)
            {
                shouldAllow = false;
            }
        }

        private void OnZombieDamaging(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            Vector3 point = parameters.zombie.transform.position;

            float damageMultiplier = _positionnalZones
                .Where(zone => zone.Collides(point))
                .Select(zone => _configuration[zone.Group].PositionnalProperties)
                .SelectMany(properties => properties.Where(property => property.Type == PositionnalZoneProperty.EType.ZombieDamage))
                .Select(property => float.Parse(property.Data))
                .Aggregate((acc, cur) => acc * cur);

            parameters.damage *= damageMultiplier;

            if (parameters.damage == 0)
            {
                shouldAllow = false;
            }
        }

        private void OnAnimalDamaging(ref DamageAnimalParameters parameters, ref bool shouldAllow)
        {
            Vector3 point = parameters.animal.transform.position;

            float damageMultiplier = _positionnalZones
                .Where(zone => zone.Collides(point))
                .Select(zone => _configuration[zone.Group].PositionnalProperties)
                .SelectMany(properties => properties.Where(property => property.Type == PositionnalZoneProperty.EType.AnimalDamage))
                .Select(property => float.Parse(property.Data))
                .Aggregate((acc, cur) => acc * cur);

            parameters.damage *= damageMultiplier;

            if (parameters.damage == 0)
            {
                shouldAllow = false;
            }
        }

        private void OnVehicleDamaging(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            float damageMultiplier = _positionnalZones
                .Where(zone => zone.Collides(vehicle.transform.position))
                .Select(zone => _configuration[zone.Group].PositionnalProperties)
                .SelectMany(properties => properties.Where(property => property.Type == PositionnalZoneProperty.EType.VehicleDamage))
                .Select(property => float.Parse(property.Data))
                .Aggregate((acc, cur) => acc * cur);

            pendingTotalDamage = ConvertDamage(pendingTotalDamage * damageMultiplier);

            if (pendingTotalDamage == 0)
            {
                shouldAllow = false;
            }
        }

        private ushort ConvertDamage(float damageToConvert)
        {
            ushort damage = (ushort)damageToConvert;

            if (Random.value < damageToConvert % 1)
                damage++;

            return damage;
        }
        #endregion

        #region PlayerEvents
        private void OnPlayerEnteredZone(object sender, Player player)
        {
            HighlightedZone zone = (HighlightedZone)sender;

            var properties = _configuration[zone.Group]
                .EventProperties
                .Where(property => property.Event == EventZoneProperty.EEvent.Enter)
                .GroupBy(property => property.Type);

            OnChatTriggered
            (
                player,
                properties.FirstOrDefault(property => property.Key == EventZoneProperty.EType.Chat)
            );

            OnWalkThroughTriggered
            (
                player,
                properties.FirstOrDefault(property => property.Key == EventZoneProperty.EType.Chat),
                zone
            );

            OnExecuteCommandTriggered
            (
                player,
                properties.FirstOrDefault(property => property.Key == EventZoneProperty.EType.Chat)
            );
        }

        private void OnPlayerExitedZone(object sender, Player player)
        {
            HighlightedZone zone = (HighlightedZone)sender;

            var properties = _configuration[zone.Group]
                .EventProperties
                .Where(property => property.Event == EventZoneProperty.EEvent.Exit)
                .GroupBy(property => property.Type);

            OnChatTriggered
            (
                player, 
                properties.FirstOrDefault(property => property.Key == EventZoneProperty.EType.Chat)
            );

            OnWalkThroughTriggered
            (
                player, 
                properties.FirstOrDefault(property => property.Key == EventZoneProperty.EType.Chat), 
                zone
            );

            OnExecuteCommandTriggered
            (
                player, 
                properties.FirstOrDefault(property => property.Key == EventZoneProperty.EType.Chat)
            );
        }

        private void OnChatTriggered(Player player, IEnumerable<EventZoneProperty> properties)
        {
            foreach (EventZoneProperty property in properties)
            {
                string text = property.Data
                    .Replace("{Player}", player.GetSteamPlayer().playerID.characterName);

                ChatManager.serverSendMessage(text, Color.white, toPlayer: player.GetSteamPlayer(), useRichTextFormatting: true);
            }
        }

        private void OnWalkThroughTriggered(Player player, IEnumerable<EventZoneProperty> properties, HighlightedZone zone)
        {
            EventZoneProperty property = properties.FirstOrDefault();

            if (property == null)
                return;
            
            Vector3 borderZonePoint = zone.Collider.ClosestPointOnBounds(player.transform.position);
            Vector3 diff = (borderZonePoint - zone.Volume.Center).normalized;
            Vector3 destination = borderZonePoint + diff;

            player.teleportToLocation(destination, player.transform.eulerAngles.y);
        }

        private void OnExecuteCommandTriggered(Player player, IEnumerable<EventZoneProperty> properties)
        {
            foreach (EventZoneProperty property in properties)
            {
                string text = property.Data
                    .Replace("{Player}", player.GetSteamPlayer().playerID.characterName)
                    .Replace("{PlayerID}", player.GetSteamID().ToString());

                _commandAdapter.Execute(text);
            }
        }
        #endregion
    }
}
