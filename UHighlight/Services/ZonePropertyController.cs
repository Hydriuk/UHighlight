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
using System.Reflection;
using System.Threading.Tasks;
using UHighlight.API;
using UHighlight.Components;
using UHighlight.Extensions;
using UHighlight.Models;
using UnityEngine;
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

        private readonly Action<PlayerMovement, object> _velocitySetter = typeof(PlayerMovement).GetField("velocity", BindingFlags.Instance | BindingFlags.NonPublic).SetValue; 

        private readonly Dictionary<string, ZoneGroup> _configuration;

        private readonly ICommandAdapter _commandAdapter;
        private readonly IPermissionAdapter _permissionAdapter;
        private readonly IVolumeStore _volumeStore;
        private readonly IHighlightSpawner _highlightSpawner;

        public ZonePropertyController(ICommandAdapter commandAdapter, IPermissionAdapter permissionAdapter, IVolumeStore volumeStore, IHighlightSpawner highlightSpawner)
        {
            _highlightSpawner = highlightSpawner;
            _volumeStore = volumeStore;
            _commandAdapter = commandAdapter;
            _permissionAdapter = permissionAdapter;
            _configuration = new Dictionary<string, ZoneGroup>();
            
            StructureManager.onDeployStructureRequested += OnStructureDeploying;
            BarricadeManager.onDeployBarricadeRequested += OnBarricadeDeploying;

            StructureManager.onDamageStructureRequested += OnBuildableDamaging;
            BarricadeManager.onDamageBarricadeRequested += OnBuildableDamaging;
            DamageTool.damagePlayerRequested += OnPlayerDamaging;
            DamageTool.damageZombieRequested += OnZombieDamaging;
            DamageTool.damageAnimalRequested += OnAnimalDamaging;
            VehicleManager.onDamageVehicleRequested += OnVehicleDamaging;

            Task.Run(InitZones);
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

        public void Refresh()
        {
            foreach (var zone in _spawnedZones)
            {
                zone.Dispose();
            }
            _spawnedZones.Clear();
            _positionnalZones.Clear();
            _configuration.Clear();

            _ = InitZones();
        }

        private async Task InitZones()
        {
            IEnumerable<ZoneGroup> groups = _volumeStore.GetGroups();

            foreach (ZoneGroup group in groups)
            {
                IEnumerable<HighlightedZone> zones = await _highlightSpawner.BuildZones(group.Name);

                _spawnedZones.AddRange(zones);

                if (group.GetPositionnalProperties().Count() > 0)
                {
                    _positionnalZones.AddRange(zones);
                }

                if (group.GetEventProperties().Count() > 0)
                {
                    foreach (var zone in zones)
                    {
                        zone.PlayerEntered += OnPlayerEnteredZone;
                        zone.PlayerExited += OnPlayerExitedZone;
                    }
                }
            }

            foreach (var item in groups.ToDictionary(group => group.Name))
            {
                _configuration.Add(item.Key, item.Value);
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
            return !_positionnalZones
                .Where(zone => zone.Collides(point))
                .Select(zone => _configuration[zone.Group].GetPositionnalProperties())
                .SelectMany(properties => properties.Where(property => property.Type == ZoneProperty.EType.PlaceStructure))
                .Any();
        }
        #endregion

        #region Damage
        private void OnBuildableDamaging(CSteamID instigatorSteamID, Transform buildableTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (damageOrigin == EDamageOrigin.Horde_Beacon_Self_Destruct ||
                damageOrigin == EDamageOrigin.Trap_Wear_And_Tear || 
                damageOrigin == EDamageOrigin.Carepackage_Timeout ||
                damageOrigin == EDamageOrigin.Plant_Harvested ||
                damageOrigin == EDamageOrigin.Charge_Self_Destruct)
                return;

            float damageMultiplier = _positionnalZones
                .Where(zone => zone.Collides(buildableTransform.position))
                .Select(zone => _configuration[zone.Group].GetPositionnalProperties())
                .SelectMany(properties => properties.Where(property => property.Type == ZoneProperty.EType.StructureDamage))
                .Select(property => float.Parse(property.Data))
                .Prepend(1) // Set base (default) value
                .Aggregate((acc, cur) => acc * cur);

            pendingTotalDamage = ConvertDamage(pendingTotalDamage * damageMultiplier);

            if (pendingTotalDamage == 0)
            {
                shouldAllow = false;
            }
        }

        private void OnPlayerDamaging(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            Vector3 point = parameters.player.transform.position;

            float damageMultiplier = _positionnalZones
                .Where(zone => zone.Collides(point))
                .Select(zone => _configuration[zone.Group].GetPositionnalProperties())
                .SelectMany(properties => properties.Where(property => property.Type == ZoneProperty.EType.PlayerDamage))
                .Select(property => float.Parse(property.Data))
                .Prepend(1) // Set base (default) value
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
                .Select(zone => _configuration[zone.Group].GetPositionnalProperties())
                .SelectMany(properties => properties.Where(property => property.Type == ZoneProperty.EType.ZombieDamage))
                .Select(property => float.Parse(property.Data))
                .Prepend(1) // Set base (default) value
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
                .Select(zone => _configuration[zone.Group].GetPositionnalProperties())
                .SelectMany(properties => properties.Where(property => property.Type == ZoneProperty.EType.AnimalDamage))
                .Select(property => float.Parse(property.Data))
                .Prepend(1) // Set base (default) value
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
                .Select(zone => _configuration[zone.Group].GetPositionnalProperties())
                .SelectMany(properties => properties.Where(property => property.Type == ZoneProperty.EType.VehicleDamage))
                .Select(property => float.Parse(property.Data))
                .Prepend(1) // Set base (default) value
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

            OnZoneCrossed(player, zone, ZoneProperty.EEvent.Enter);
        }

        private void OnPlayerExitedZone(object sender, Player player)
        {
            HighlightedZone zone = (HighlightedZone)sender;

            OnZoneCrossed(player, zone, ZoneProperty.EEvent.Exit);
        }

        private void OnZoneCrossed(Player player, HighlightedZone zone, ZoneProperty.EEvent eventType)
        {
            var properties = _configuration[zone.Group]
                .GetEventProperties()
                .Where(property => property.Event == eventType)
                .GroupBy(property => property.Type);

            OnChatTriggered
            (
                player, zone,
                properties.FirstOrDefault(property => property.Key == ZoneProperty.EType.Chat)
            );

            OnRepulseTriggered
            (
                player, zone,
                properties.FirstOrDefault(property => property.Key == ZoneProperty.EType.Repulse)
            );

            OnExecuteCommandTriggered
            (
                player, zone,
                properties.FirstOrDefault(property => property.Key == ZoneProperty.EType.ExecuteCommand)
            );

            OnGivePermissionTriggered
            (
                player, zone,
                properties.FirstOrDefault(property => property.Key == ZoneProperty.EType.GivePermissionGroup)
            );

            OnRemovePermissionTriggered
            (
                player, zone,
                properties.FirstOrDefault(property => property.Key == ZoneProperty.EType.RemovePermissionGroup)
            );
        }

        private void OnChatTriggered(Player player, HighlightedZone zone, IEnumerable<ZoneProperty> properties)
        {
            if (properties == null)
                return;

            foreach (ZoneProperty property in properties)
            {
                string text = FormatText(property.Data, player, zone);

                ChatManager.serverSendMessage(text, Color.white, toPlayer: player.GetSteamPlayer(), useRichTextFormatting: true);
            }
        }

        private void OnRepulseTriggered(Player player, HighlightedZone zone, IEnumerable<ZoneProperty> properties)
        {
            if (properties == null)
                return;

            ZoneProperty property = properties.FirstOrDefault();

            if (property == null)
                return;
            
            Vector3 borderZonePoint = zone.Collider.ClosestPointOnBounds(player.transform.position);
            Vector3 diff = (borderZonePoint - zone.Volume.Center).normalized;
            Vector3 destination = borderZonePoint;

            if (!float.TryParse(property.Data, out float repultionForce))
                return;

            _velocitySetter(player.movement, diff * repultionForce);
        }

        private void OnExecuteCommandTriggered(Player player, HighlightedZone zone, IEnumerable<ZoneProperty> properties)
        {
            if (properties == null)
                return;
            
            foreach (ZoneProperty property in properties)
            {
                string text = FormatText(property.Data, player, zone);

                _commandAdapter.Execute(text);
            }
        }

        private void OnGivePermissionTriggered(Player player, HighlightedZone zone, IEnumerable<ZoneProperty> properties)
        {
            if (properties == null)
                return;

            foreach (ZoneProperty property in properties)
            {
                string permissionGroup = FormatText(property.Data, player, zone);

                _permissionAdapter.AddToGroup(player.GetSteamID(), permissionGroup);
            }
        }

        private void OnRemovePermissionTriggered(Player player, HighlightedZone zone, IEnumerable<ZoneProperty> properties)
        {
            if (properties == null)
                return;

            foreach (ZoneProperty property in properties)
            {
                string permissionGroup = FormatText(property.Data, player, zone);

                _permissionAdapter.RemoveFromGroup(player.GetSteamID(), permissionGroup);
            }
        }
        #endregion

        private string FormatText(string text, Player player, HighlightedZone zone)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text
                .Replace("{Player}", player.GetSteamPlayer().playerID.characterName)
                .Replace("{PlayerID}", player.GetSteamID().ToString())
                .Replace("{ZoneName}", zone.Name);
        }
    }
}
