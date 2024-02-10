#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using Hydriuk.UnturnedModules.Adapters;
using Hydriuk.UnturnedModules.Extensions;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using UHighlight.API;
using UHighlight.Extensions;
using UHighlight.Models;

namespace UHighlight.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    internal class AdminUIManager : IAdminUIManager
    {
        private const int GROUPS_PAGE_SIZE = 7;
        private const int ZONES_PAGE_SIZE = 8;
        private const int PROPERTIES_PAGE_SIZE = 6;

        private readonly ushort _effectID = 21700;
        private short _effectKey { get => (short)(_effectID + short.MaxValue); }

        private readonly Dictionary<Player, PlayerData> _playersData = new Dictionary<Player, PlayerData>();

        private readonly IVolumeEditor _volumeEditor;
        private readonly IVolumeStore _volumeStore;
        private readonly IEffectBuilder _effectBuilder;
        private readonly IThreadAdapter _threadAdapter;
        private readonly IZonePropertyController _propertyController;

        public AdminUIManager(
            IVolumeEditor volumeEditor, 
            IVolumeStore volumeStore, 
            IEffectBuilder effectBuilder, 
            IThreadAdapter threadAdapter,
            IZonePropertyController propertyController
            ) 
        {
            _volumeEditor = volumeEditor;
            _volumeStore = volumeStore;
            _effectBuilder = effectBuilder;
            _threadAdapter = threadAdapter;
            _propertyController = propertyController;

            EffectManager.onEffectButtonClicked += OnButtonClicked;
            EffectManager.onEffectTextCommitted += OnTextCommitted;
        }

        public void Dispose()
        {
            EffectManager.onEffectButtonClicked -= OnButtonClicked;
            EffectManager.onEffectTextCommitted -= OnTextCommitted;

            _threadAdapter.RunOnMainThread(() =>
            {
                foreach (var player in _playersData.Keys)
                {
                    EffectManager.askEffectClearByID(_effectID, player.GetTransportConnection());
                }
            });
        }

        private void OnButtonClicked(Player player, string buttonName)
        {
            if (!_playersData.TryGetValue(player, out PlayerData playerData))
                return;

            switch (buttonName)
            {
                // General
                case "CloseUHighlightUIButton":
                    HideUI(player);
                    player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);
                    break;
                case "CaptureCursorButton":
                    player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
                    break;
                case "ReleaseCursorButton":
                    player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);
                    break;

                // Group
                case "GroupsPreviousPageButton":
                    playerData.SelectedGroupsPage = Math.Max(0, playerData.SelectedGroupsPage - 1);
                    UpdateUI(player);
                    break;
                case "GroupsNextPageButton":
                    playerData.SelectedGroupsPage = Math.Min((_volumeStore.GetGroups().Count()-1) / GROUPS_PAGE_SIZE, playerData.SelectedGroupsPage + 1);
                    UpdateUI(player);
                    break;

                // Zones
                case "ZonesPreviousPageButton":
                    playerData.SelectedZonesPage = Math.Max(0, playerData.SelectedZonesPage - 1);
                    UpdateUI(player);
                    break;
                case "ZonesNextPageButton":
                    playerData.SelectedZonesPage = Math.Min((_volumeStore.GetVolumes(playerData.SelectedGroup).Count()-1) / ZONES_PAGE_SIZE, playerData.SelectedZonesPage + 1);
                    UpdateUI(player);
                    break;
                case "StartZoneCreationButton":
                    _volumeEditor.StartEditing(player, playerData.SelectedShape, playerData.SelectedMaterial, playerData.SelectedColor);
                    player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);
                    EffectManager.sendUIEffectVisibility(_effectKey, player.GetTransportConnection(), true, $"CaptureCursorButton", true);
                    break;
                case "ValidateZoneCreationButton":
                    if(playerData.CreatingZoneName != string.Empty)
                        _volumeEditor.Validate(player, playerData.SelectedGroup, playerData.CreatingZoneName);
                    UpdateUI(player);
                    ResetZones();
                    break;
                case "CancelZoneCreationButton":
                    _volumeEditor.StopEditing(player);
                    break;
                case "CreateZoneGroupButton":
                    _volumeStore.CreateGroup(playerData.CreatingGroupName);
                    UpdateUI(player);
                    break;
                case "HideZones":
                    _effectBuilder.KillAllEffects(player);
                    break;
                // Zone shape
                case "ZoneSetCubeButton":
                    playerData.SelectedShape = EVolumeShape.Cube;
                    break;
                case "ZoneSetSphereButton":
                    playerData.SelectedShape = EVolumeShape.Sphere;
                    break;
                // Zone material
                case "ZoneSetSolidButton":
                    playerData.SelectedMaterial = "Solid";
                    break;
                case "ZoneSetTransparentButton":
                    playerData.SelectedMaterial = "Transparent";
                    break;
                // Zone color
                case "ZoneSetRedButton":
                    playerData.SelectedColor = "Red";
                    break;
                case "ZoneSetGreenButton":
                    playerData.SelectedColor = "Green";
                    break;
                case "ZoneSetBlueButton":
                    playerData.SelectedColor = "Blue";
                    break;
                case "ZoneSetMagentaButton":
                    playerData.SelectedColor = "Magenta";
                    break;
                case "ZoneSetLimeButton":
                    playerData.SelectedColor = "Lime";
                    break;
                case "ZoneSetCyanButton":
                    playerData.SelectedColor = "Cyan";
                    break;
                case "ZoneSetGoldButton":
                    playerData.SelectedColor = "Gold";
                    break;
                case "ZoneSetSilverButton":
                    playerData.SelectedColor = "Silver";
                    break;
                case "ZoneSetCopperButton":
                    playerData.SelectedColor = "Copper";
                    break;
                case "ZoneSetPinkButton":
                    playerData.SelectedColor = "Pink";
                    break;

                // Properties
                case "PropertiesPreviousPageButton":
                    playerData.SelectedPropertiesPage = Math.Max(0, playerData.SelectedPropertiesPage - 1);
                    UpdateUI(player);
                    break;
                case "PropertiesNextPageButton":
                    playerData.SelectedPropertiesPage = Math.Min
                    (
                        (_volumeStore.GetGroup(playerData.SelectedGroup).Properties.Count() - 1) / PROPERTIES_PAGE_SIZE, 
                        playerData.SelectedPropertiesPage + 1
                    );
                    UpdateUI(player);
                    break;
                case "CreatePropertyButton":
                    _volumeStore.CreateProperty(playerData.SelectedGroup, new ZoneProperty() 
                    { 
                        Type = playerData.CreatingPropertyType, 
                        Event = playerData.CreatingPropertyEvent, 
                        Data = playerData.CreatingPropertyData 
                    });
                    ResetProperty(player);
                    UpdateUI(player);
                    ResetZones();
                    break;
                case "ClosePropertiesButton":
                    ResetProperty(player);
                    break;
                // Property event
                case "SetEnterEventButton":
                    playerData.CreatingPropertyEvent = ZoneProperty.EEvent.Enter;
                    break;
                case "SetExitEventButton":
                    playerData.CreatingPropertyEvent = ZoneProperty.EEvent.Exit;
                    break;
                // Property action
                case "SetPlaceStructurePropertyButton":
                    playerData.CreatingPropertyType = ZoneProperty.EType.PlaceStructure;
                    break;
                case "SetStructureDamagePropertyButton":
                    playerData.CreatingPropertyType = ZoneProperty.EType.StructureDamage;
                    break;
                case "SetPlayerDamagePropertyButton":
                    playerData.CreatingPropertyType = ZoneProperty.EType.PlayerDamage;
                    break;
                case "SetVehicleDamagePropertyButton":
                    playerData.CreatingPropertyType = ZoneProperty.EType.VehicleDamage;
                    break;
                case "SetZombieDamagePropertyButton":
                    playerData.CreatingPropertyType = ZoneProperty.EType.ZombieDamage;
                    break;
                case "SetAnimalDamagePropertyButton":
                    playerData.CreatingPropertyType = ZoneProperty.EType.AnimalDamage;
                    break;
                case "SetGivePermissionPropertyButton":
                    playerData.CreatingPropertyType = ZoneProperty.EType.GivePermissionGroup;
                    break;
                case "SetRemovePermissionPropertyButton":
                    playerData.CreatingPropertyType = ZoneProperty.EType.RemovePermissionGroup;
                    break;
                case "SetChatPropertyButton":
                    playerData.CreatingPropertyType = ZoneProperty.EType.Chat;
                    break;
                case "SetCommandPropertyButton":
                    playerData.CreatingPropertyType = ZoneProperty.EType.ExecuteCommand;
                    break;
                case "SetWalkThroughPropertyButton":
                    playerData.CreatingPropertyType = ZoneProperty.EType.WalkThrough;
                    break;

                default:
                    OnTemplatedButtonClicked(player, buttonName, playerData);
                    break;
            }
        }

        private void OnTemplatedButtonClicked(Player player, string buttonName, PlayerData playerData)
        {
            Match indexMatch = Regex.Match(buttonName, @"\((?<index>\d)\)");

            if (!int.TryParse(indexMatch.Groups["index"].Value, out int index))
                return;

            if (Regex.IsMatch(buttonName, @"SelectGroup \(\d\)"))
            {
                playerData.SelectedGroup = playerData.DisplayedGroups[index];
                UpdateUI(player);
            }
            else if (Regex.IsMatch(buttonName, @"ShowGroup \(\d\)"))
            {
                _effectBuilder.DisplayGroupEffects(playerData.DisplayedGroups[index]);
            }
            else if (Regex.IsMatch(buttonName, @"DeleteGroup \(\d\)"))
            {
                _volumeStore.DeleteGroup(playerData.DisplayedGroups[index]);
                UpdateUI(player);
                ResetZones();
            }
            else if (Regex.IsMatch(buttonName, @"ShowZone \(\d\)"))
            {
                _effectBuilder.DisplayEffect(playerData.DisplayedZones[index], player);
            }
            else if (Regex.IsMatch(buttonName, @"DeleteZone \(\d\)"))
            {
                _volumeStore.DeleteVolume(playerData.SelectedGroup, playerData.DisplayedZones[index].Name);
                UpdateUI(player);
                ResetZones();
            }
            else if(Regex.IsMatch(buttonName, @"DeleteProperty \(\d\)"))
            {
                _volumeStore.DeleteProperty(playerData.SelectedGroup, index);
                UpdateUI(player);
                ResetZones();
            }
        }

        private void OnTextCommitted(Player player, string buttonName, string text)
        {
            if (!_playersData.TryGetValue(player, out PlayerData playerData))
                return;

            switch (buttonName)
            {
                case "NewGroupNameInput":
                    playerData.CreatingGroupName = text;
                    break;

                case "NewZoneNameInput":
                    playerData.CreatingZoneName = text;
                    break;

                case "PropertyDataInput":
                    playerData.CreatingPropertyData = text;
                    break;

                default:
                    OnTemplatedTextCommitted(player, buttonName, text, playerData);
                    break;
            }
        }

        private void OnTemplatedTextCommitted(Player player, string buttonName, string text, PlayerData playerData)
        {
        }

        private void ResetProperty(Player player)
        {
            if (!_playersData.TryGetValue(player, out PlayerData playerData))
                return;

            playerData.CreatingPropertyType = ZoneProperty.EType.PlaceStructure;
            playerData.CreatingPropertyEvent = ZoneProperty.EEvent.Enter;
            playerData.CreatingPropertyData = string.Empty;

            EffectManager.sendUIEffectVisibility(_effectKey, player.GetTransportConnection(), true, $"ResetHighlightProperty", true);
        }

        public void ShowUI(Player player)
        {
            if (_playersData.TryGetValue(player, out PlayerData playerData))
                return;

            playerData = new PlayerData()
            {
                SelectedShape = EVolumeShape.Cube,
                SelectedMaterial = "Transparent",
                SelectedColor = "Blue"
            };

            _playersData.Add(player, playerData);

            _threadAdapter.RunOnMainThread(() =>
            {
                EffectManager.sendUIEffect(_effectID, _effectKey, player.GetTransportConnection(), true);

                player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);

                UpdateUI(player);
            });
        }

        public void HideUI(Player player)
        {
            _playersData.Remove(player);

            _threadAdapter.RunOnMainThread(() =>
            {
                EffectManager.askEffectClearByID(_effectID, player.GetTransportConnection());
            });
        }

        private void UpdateUI(Player player)
        {
            if (!_playersData.TryGetValue(player, out PlayerData playerData))
                return;

            List<string> groups = _volumeStore.GetGroupNames().ToList();

            int groupPageCount = (groups.Count - 1) / GROUPS_PAGE_SIZE + 1;

            EffectManager.sendUIEffectText(_effectKey, player.GetTransportConnection(), true, "GroupsPage", $"{playerData.SelectedGroupsPage + 1}/{groupPageCount}");

            playerData.DisplayedGroups = groups
                .Skip(playerData.SelectedGroupsPage * GROUPS_PAGE_SIZE)
                .Take(GROUPS_PAGE_SIZE)
                .ToList();

            for (int i = 0; i < GROUPS_PAGE_SIZE; i++) 
            {
                if (playerData.DisplayedGroups.Count > i)
                {
                    EffectManager.sendUIEffectText(_effectKey, player.GetTransportConnection(), true, $"GroupName ({i})", playerData.DisplayedGroups[i]);
                }

                EffectManager.sendUIEffectVisibility(_effectKey, player.GetTransportConnection(), true, $"Group ({i})", playerData.DisplayedGroups.Count > i);
            }

            if (playerData.SelectedGroup != string.Empty)
            {
                List<Volume> zones = _volumeStore.GetVolumes(playerData.SelectedGroup).ToList();

                int zonePageCount = (zones.Count - 1) / ZONES_PAGE_SIZE + 1;

                EffectManager.sendUIEffectText(_effectKey, player.GetTransportConnection(), true, "ZonesTitle", $"{playerData.SelectedGroup}'s zones");
                EffectManager.sendUIEffectText(_effectKey, player.GetTransportConnection(), true, "ZonesPage", $"{playerData.SelectedZonesPage + 1}/{zonePageCount}");

                EffectManager.sendUIEffectVisibility(_effectKey, player.GetTransportConnection(), true, $"Zones", true);

                playerData.DisplayedZones = zones
                    .Skip(playerData.SelectedZonesPage * ZONES_PAGE_SIZE)
                    .Take(ZONES_PAGE_SIZE)
                    .ToList();

                for (int i = 0; i < ZONES_PAGE_SIZE; i++)
                {
                    if (playerData.DisplayedZones.Count > i)
                    {
                        EffectManager.sendUIEffectText(_effectKey, player.GetTransportConnection(), true, $"ZoneName ({i})", playerData.DisplayedZones[i].Name);
                    }

                    EffectManager.sendUIEffectVisibility(_effectKey, player.GetTransportConnection(), true, $"Zone ({i})", playerData.DisplayedZones.Count > i);
                }
            }
            else
            {
                EffectManager.sendUIEffectVisibility(_effectKey, player.GetTransportConnection(), true, $"Zones", false);
            }

            if(playerData.SelectedGroup != string.Empty)
            {
                List<ZoneProperty> properties = _volumeStore.GetProperties(playerData.SelectedGroup);

                int propertiesPageCount = (properties.Count - 1) / PROPERTIES_PAGE_SIZE + 1;

                EffectManager.sendUIEffectText(_effectKey, player.GetTransportConnection(), true, "PropertiesPage", $"{playerData.SelectedPropertiesPage + 1}/{propertiesPageCount}");

                playerData.DisplayedProperties = properties
                    .Skip(playerData.SelectedPropertiesPage * PROPERTIES_PAGE_SIZE)
                    .Take(PROPERTIES_PAGE_SIZE)
                    .ToList();

                for (int i = 0; i < PROPERTIES_PAGE_SIZE; i++)
                {
                    if(playerData.DisplayedProperties.Count > i)
                    {
                        EffectManager.sendUIEffectText(_effectKey, player.GetTransportConnection(), true, $"PropertyName ({i})", playerData.DisplayedProperties[i].Type.ToString());
                        if (playerData.DisplayedProperties[i].IsEventProperty())
                        {
                            if(playerData.DisplayedProperties[i].Event == ZoneProperty.EEvent.Enter)
                                EffectManager.sendUIEffectVisibility(_effectKey, player.GetTransportConnection(), true,  $"PropertyEnterEvent ({i})", true);
                            else
                                EffectManager.sendUIEffectVisibility(_effectKey, player.GetTransportConnection(), true,  $"PropertyExitEvent ({i})", true);
                        }
                        else
                        {
                            EffectManager.sendUIEffectVisibility(_effectKey, player.GetTransportConnection(), true, $"ResetPropertyEvent ({i})", true);
                        }

                        EffectManager.sendUIEffectText(_effectKey, player.GetTransportConnection(), true, $"PropertyData ({i})", playerData.DisplayedProperties[i].Data);
                    }

                    EffectManager.sendUIEffectVisibility(_effectKey, player.GetTransportConnection(), true, $"Property ({i})", playerData.DisplayedProperties.Count > i);
                }
            }
        }

        private void ResetZones()
        {
            _propertyController.Refresh();
        }

        private class PlayerData
        {
            public int SelectedGroupsPage { get; set; }
            public string SelectedGroup { get; set; } = string.Empty;
            public int SelectedZonesPage { get; set; }
            public string SelectedZone { get; set; } = string.Empty;
            public int SelectedPropertiesPage { get; set; }


            public EVolumeShape SelectedShape { get; set; }
            public string SelectedMaterial { get; set; } = string.Empty;
            public string SelectedColor { get; set; } = string.Empty;

            public string CreatingGroupName { get; set; } = string.Empty;
            public string CreatingZoneName { get; set; } = string.Empty;

            public ZoneProperty.EType CreatingPropertyType { get; set; }
            public ZoneProperty.EEvent CreatingPropertyEvent { get; set; }
            public string CreatingPropertyData { get; set; } = string.Empty;

            public List<string> DisplayedGroups { get; set; } = new List<string>();
            public List<Volume> DisplayedZones { get; set; } = new List<Volume>();
            public List<ZoneProperty> DisplayedProperties { get; set; } = new List<ZoneProperty>();
        }
    }
}
