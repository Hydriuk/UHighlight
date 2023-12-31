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
using UHighlight.API;
using UHighlight.Components;
using UHighlight.DAL;
using UHighlight.Extensions;
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

        private readonly Dictionary<string, Configuration.Effect> _deployStructureGroups = new Dictionary<string, Configuration.Effect>();
        private readonly Dictionary<string, Configuration.Effect> _damageStructureGroups = new Dictionary<string, Configuration.Effect>();
        private readonly Dictionary<string, Configuration.Effect> _damagePlayerGroups = new Dictionary<string, Configuration.Effect>();
        private readonly Dictionary<string, Configuration.Effect> _damageZombieGroups = new Dictionary<string, Configuration.Effect>();
        private readonly Dictionary<string, Configuration.Effect> _damageAnimalGroups = new Dictionary<string, Configuration.Effect>();
        private readonly Dictionary<string, Configuration.Effect> _damageVehicleGroups = new Dictionary<string, Configuration.Effect>();
        private readonly Dictionary<string, List<Configuration.Effect>> _permissionGroups = new Dictionary<string, List<Configuration.Effect>>();

        private readonly Dictionary<string, List<Configuration.Effect>> _chatGroups = new Dictionary<string, List<Configuration.Effect>>();
        private readonly Dictionary<string, List<Configuration.Effect>> _walkThroughGroups = new Dictionary<string, List<Configuration.Effect>>();
        private readonly Dictionary<string, List<Configuration.Effect>> _executeCommandGroups = new Dictionary<string, List<Configuration.Effect>>();

        private readonly ICommandAdapter _commandAdapter;
        private readonly IVolumeStore _volumeStore;

        public ZonePropertyController(IHighlightSpawner highlightSpawner, ICommandAdapter commandAdapter, IVolumeStore volumeStore)
        {
            _commandAdapter = commandAdapter;
            _volumeStore = volumeStore;

            volumeStore.GetGroups();

            Console.WriteLine("IconURL");
            //Console.WriteLine(configuration.Configuration.IconUrl);

            // Build configurations
            //_deployStructureGroups = CreateConfiguration(configuration.Configuration, Configuration.EEventEffect.PlaceStructure);
            //_damageStructureGroups = CreateConfiguration(configuration.Configuration, Configuration.EEventEffect.StructureDamage);
            //_damagePlayerGroups = CreateConfiguration(configuration.Configuration, Configuration.EEventEffect.PlayerDamage);
            //_damageZombieGroups = CreateConfiguration(configuration.Configuration, Configuration.EEventEffect.ZombieDamage);
            //_damageAnimalGroups = CreateConfiguration(configuration.Configuration, Configuration.EEventEffect.AnimalDamage);
            //_damageVehicleGroups = CreateConfiguration(configuration.Configuration, Configuration.EEventEffect.VehicleDamage);
            //_permissionGroups = CreateListConfiguration(configuration.Configuration, Configuration.EEventEffect.PermissionGroup);

            //_chatGroups = CreateListConfiguration(configuration.Configuration, Configuration.EEventEffect.Chat);
            //_walkThroughGroups = CreateListConfiguration(configuration.Configuration, Configuration.EEventEffect.WalkThrough);
            //_executeCommandGroups = CreateListConfiguration(configuration.Configuration, Configuration.EEventEffect.ExecuteCommand);


            //_globalConfiguration = configuration.Configuration.GlobalEffects;
            //Console.WriteLine(configuration.Configuration.GroupEffects.Count);
            //foreach (var group in configuration.Configuration.GroupEffects.Select(group => group.Name))
            //{
            //    _spawnedZones.AddRange(highlightSpawner.BuildZones(group));
            //}

            StructureManager.onDeployStructureRequested += OnStructureDeploying;
            BarricadeManager.onDeployBarricadeRequested += OnBarricadeDeploying;

            StructureManager.onDamageStructureRequested += OnBuildableDamaging;
            BarricadeManager.onDamageBarricadeRequested += OnBuildableDamaging;
            DamageTool.damagePlayerRequested += OnPlayerDamaging;
            DamageTool.damageZombieRequested += OnZombieDamaging;
            DamageTool.damageAnimalRequested += OnAnimalDamaging;
            VehicleManager.onDamageVehicleRequested += OnVehicleDamaging;

            foreach (HighlightedZone zone in _spawnedZones)
            {
                Console.WriteLine(zone.Group);
                if (_chatGroups.ContainsKey(zone.Group))
                {
                    Console.WriteLine("Chat");
                    zone.PlayerEntered += OnEnteredChat;
                    zone.PlayerExited += OnExitedChat;
                }

                if (_walkThroughGroups.ContainsKey(zone.Group))
                {
                    zone.PlayerEntered += OnEnteredWalkThrough;
                    zone.PlayerExited += OnExitedWalkThrough;
                }


                if (_executeCommandGroups.ContainsKey(zone.Group))
                {
                    zone.PlayerEntered += OnEnteredExecuteCommand;
                    zone.PlayerExited += OnExitedExecuteCommand;
                }
            }
        }

        #region Configuration
        private Dictionary<string, Configuration.Effect> CreateConfiguration(Configuration configuration, Configuration.EEventEffect effectType)
        {
            return configuration.GroupEffects
                .Select(group =>
                (
                    group.Name,
                    group.Effects
                        .FirstOrDefault(effect => effect.Type == effectType)
                ))
                .Where(group => group.Item2 != null)
                .ToDictionary(group => group.Name, group => group.Item2);
        }

        private Dictionary<string, List<Configuration.Effect>> CreateListConfiguration(Configuration configuration, Configuration.EEventEffect effectType)
        {
            return configuration.GroupEffects
                .Select(group =>
                (
                    group.Name,
                    group.Effects
                        .Where(effect => effect.Type == effectType)
                ))
                .Where(group => group.Item2.Count() > 0)
                .ToDictionary(group => group.Name, group => group.Item2.ToList());
        }
        #endregion

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
            return GetBoolValue
            (
                _deployStructureGroups,
                Configuration.EEventEffect.PlaceStructure,
                point,
                true
            );
        }
        #endregion

        //private void OnEnterPermission(object sender, Player player)
        //{
        //    HighlightedZone zone = (HighlightedZone)sender;

        //}

        //private void OnExitPermission(object sender, Player player)
        //{
        //    HighlightedZone zone = (HighlightedZone)sender;

        //}

        #region Damage
        private void OnBuildableDamaging(CSteamID instigatorSteamID, Transform buildableTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            float damageMultiplier = GetFloatValue
            (
                _damageStructureGroups,
                Configuration.EEventEffect.StructureDamage,
                buildableTransform.position,
                1
            );

            pendingTotalDamage = ConvertDamage(pendingTotalDamage * damageMultiplier);

            if(pendingTotalDamage == 0)
            {
                shouldAllow = false;
            }
        }

        private void OnPlayerDamaging(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            float damageMultiplier = GetFloatValue
            (
                _damagePlayerGroups,
                Configuration.EEventEffect.PlayerDamage,
                parameters.player.transform.position,
                1
            );

            parameters.damage *= damageMultiplier;

            if (parameters.damage == 0)
            {
                shouldAllow = false;
            }
        }

        private void OnZombieDamaging(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            float damageMultiplier = GetFloatValue
            (
                _damageZombieGroups,
                Configuration.EEventEffect.ZombieDamage,
                parameters.zombie.transform.position,
                1
            );

            parameters.damage *= damageMultiplier;

            if (parameters.damage == 0)
            {
                shouldAllow = false;
            }
        }

        private void OnAnimalDamaging(ref DamageAnimalParameters parameters, ref bool shouldAllow)
        {
            float damageMultiplier = GetFloatValue
            (
                _damageAnimalGroups,
                Configuration.EEventEffect.AnimalDamage,
                parameters.animal.transform.position,
                1
            );

            parameters.damage *= damageMultiplier;

            if (parameters.damage == 0)
            {
                shouldAllow = false;
            }
        }

        private void OnVehicleDamaging(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            float damageMultiplier = GetFloatValue
            (
                _damageVehicleGroups, 
                Configuration.EEventEffect.VehicleDamage, 
                vehicle.transform.position,
                1
            );

            pendingTotalDamage = ConvertDamage(pendingTotalDamage * damageMultiplier);

            if (pendingTotalDamage == 0)
            {
                shouldAllow = false;
            }
        }

        private float GetFloatValue(Dictionary<string, Configuration.Effect> configuration, Configuration.EEventEffect eventEffect, Vector3 position, float defaultValue)
        {
            HighlightedZone zone = _spawnedZones
                .Where(zone => configuration.ContainsKey(zone.Group))
                .Where(zone => zone.Collides(position))
                .FirstOrDefault();

            float value;
            if (zone == null)
            {
                value = defaultValue;
            }
            else
            {
                value = float.Parse(configuration[zone.Group].Data);
            }

            return value;
        }

        private bool GetBoolValue(Dictionary<string, Configuration.Effect> configuration, Configuration.EEventEffect eventEffect, Vector3 position, bool defaultValue)
        {
            HighlightedZone zone = _spawnedZones
                .Where(zone => configuration.ContainsKey(zone.Group))
                .Where(zone => zone.Collides(position))
                .FirstOrDefault();

            bool value;
            if (zone == null)
            {
                //Configuration.Effect effect = _globalConfiguration
                //    .FirstOrDefault(effect => effect.Type == eventEffect);

                //if (effect == null)
                    value = defaultValue;
                //else
                //    value = bool.Parse(effect.Data);
            }
            else
            {
                value = bool.Parse(configuration[zone.Group].Data);
            }

            return value;
        }

        private ushort ConvertDamage(float damageToConvert)
        {
            ushort damage = (ushort)damageToConvert;

            if (Random.value < damageToConvert % 1)
                damage++;

            return damage;
        }
        #endregion

        #region Chat
        private void OnEnteredChat(object sender, Player player)
        {
            HighlightedZone zone = (HighlightedZone)sender;

            SendChat(zone, player, Configuration.EZoneEvent.Enter);
        }

        private void OnExitedChat(object sender, Player player)
        {
            HighlightedZone zone = (HighlightedZone)sender;

            SendChat(zone, player, Configuration.EZoneEvent.Exit);
        }

        private void SendChat(HighlightedZone zone, Player player, Configuration.EZoneEvent zoneEvent)
        {
            List<Configuration.Effect> effects = _chatGroups[zone.Group];

            foreach (Configuration.Effect effect in effects.Where(effect => effect.Event == zoneEvent))
            {
                string text = effect.Data
                    .Replace("{Player}", player.GetSteamPlayer().playerID.characterName);

                ChatManager.serverSendMessage(text, Color.white, toPlayer: player.GetSteamPlayer(), useRichTextFormatting: true);
            }
        }
        #endregion

        #region WalkThrough
        private void OnEnteredWalkThrough(object sender, Player player)
        {
            HighlightedZone zone = (HighlightedZone)sender;

            PreventWalkThrough(zone, player, Configuration.EZoneEvent.Enter);
        }

        private void OnExitedWalkThrough(object sender, Player player)
        {
            HighlightedZone zone = (HighlightedZone)sender;

            PreventWalkThrough(zone, player, Configuration.EZoneEvent.Exit);
        }

        private void PreventWalkThrough(HighlightedZone zone, Player player, Configuration.EZoneEvent zoneEvent)
        {
            List<Configuration.Effect> effects = _walkThroughGroups[zone.Group];

            Configuration.Effect effect = effects.FirstOrDefault(effect => effect.Event == zoneEvent);

            if (effect != null)
            {
                Vector3 borderZonePoint = zone.Collider.ClosestPointOnBounds(player.transform.position);
                Vector3 diff = (borderZonePoint - zone.Volume.Center).normalized;
                Vector3 destination = zoneEvent == Configuration.EZoneEvent.Enter ? borderZonePoint + diff : borderZonePoint - diff;

                player.teleportToLocation(destination, player.transform.eulerAngles.y);
            }
        }
        #endregion

        #region ExecuteCommand
        private void OnEnteredExecuteCommand(object sender, Player player)
        {
            HighlightedZone zone = (HighlightedZone)sender;

            ExecuteCommand(zone, player, Configuration.EZoneEvent.Enter);
        }

        private void OnExitedExecuteCommand(object sender, Player player)
        {
            HighlightedZone zone = (HighlightedZone)sender;

            ExecuteCommand(zone, player, Configuration.EZoneEvent.Exit);
        }

        private void ExecuteCommand(HighlightedZone zone, Player player, Configuration.EZoneEvent zoneEvent)
        {
            List<Configuration.Effect> effects = _executeCommandGroups[zone.Group];

            foreach (Configuration.Effect effect in effects.Where(effect => effect.Event == zoneEvent))
            {
                string text = effect.Data
                    .Replace("{Player}", player.GetSteamPlayer().playerID.characterName)
                    .Replace("{PlayerID}", player.GetSteamID().ToString());

                _commandAdapter.Execute(text);
            }
        }
        #endregion
    }
}
