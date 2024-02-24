using Hydriuk.UnturnedModules.Adapters;
using Hydriuk.UnturnedModules.Extensions;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Provider;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Globalization;
using UHighlight.API;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class EffectBuilder : IEffectBuilder
    {
        private readonly Dictionary<string, Guid> _effectGUIDProvider = new Dictionary<string, Guid>();

        private readonly HashSet<string> _shapes = new HashSet<string>();
        private readonly HashSet<string> _colors = new HashSet<string>();
        private readonly HashSet<string> _materials = new HashSet<string>();

        private readonly IThreadAdapter _threadAdapter;
        private readonly IVolumeStore _volumeStore;

        public EffectBuilder(IThreadAdapter threadAdapter, IVolumeStore volumeStore)
        {
            _threadAdapter = threadAdapter;
            _volumeStore = volumeStore;

            if (Level.isLoaded)
                LateLoad();
            else
                Level.onPostLevelLoaded += OnLevelLoaded;
        }

        public void Dispose()
        {
            Level.onPostLevelLoaded -= OnLevelLoaded;
        }

        private void OnLevelLoaded(int level) => LateLoad();
        private void LateLoad()
        {
            AssetOrigin assetOrigin = TempSteamworksWorkshop.FindOrAddOrigin(3006499456);

            foreach (Asset asset in assetOrigin.GetAssets())
            {
                if (asset is not EffectAsset effectAsset)
                    continue;

                string[] split = effectAsset.name.Split('_');

                if (split.Length != 3)
                    continue;

                _shapes.Add(split[0]);
                _colors.Add(split[1]);
                _materials.Add(split[2]);

                _effectGUIDProvider.Add(effectAsset.name, effectAsset.GUID);
            }
        }

        public IEnumerable<string> GetShapes() => _shapes;
        public IEnumerable<string> GetColors() => _colors;
        public IEnumerable<string> GetMaterials() => _materials;

        public bool Exists(string shape, string color, string material)
        {
            shape = ToTitleCase(shape);
            color = ToTitleCase(color);
            material = ToTitleCase(material);

            material = material switch
            {
                "T" => "Transparent",
                "S" => "Solid",
                _ => material
            };

            color = color switch
            {
                "R" => "Red",
                "Re" => "Red",
                "G" => "Green",
                "Gr" => "Green",
                "B" => "Blue",
                "Bl" => "Blue",
                "C" => "Cyan",
                "Cy" => "Cyan",
                "M" => "Magenta",
                "Ma" => "Magenta",
                "L" => "Lime",
                "Li" => "Lime",
                "Go" => "Gold",
                "S" => "Silver",
                "Si" => "Silver",
                "Co" => "Copper",
                "P" => "Pink",
                "Pi" => "Pink",
                _ => color
            };

            return _effectGUIDProvider.ContainsKey($"{shape}_{material}_{color}");
        }

        public void DisplayGroupEffects(string group, bool unique = false)
        {
            foreach (var volume in _volumeStore.GetVolumes(group))
            {
                DisplayEffect(volume, unique);
            };
        }

        public void DisplayGroupEffects(string group, Player player, bool unique = false)
        {
            foreach (var volume in _volumeStore.GetVolumes(group))
            {
                DisplayEffect(volume, player, unique);
            };
        }

        public void DisplayEffect(Volume volume, bool unique = false)
        {
            TriggerEffectParameters effectParams = BuildEffect(volume);

            ShowEffect(effectParams, unique);
        }

        public void DisplayEffect(Volume volume, Player player, bool unique = false, float customSize = -1)
        {
            TriggerEffectParameters effectParams = BuildEffect(volume);

            if(customSize != -1)
            {
                effectParams.scale = Vector3.one * customSize;
            }

            effectParams.SetRelevantPlayer(player.GetTransportConnection());

            ShowEffect(effectParams, unique);
        }

        public void KillAllEffects(Player player)
        {
            foreach (Guid guid in _effectGUIDProvider.Values)
            {
                KillEffect(guid, player);
            }
        }

        public void KillAllEffects()
        {
            foreach (Guid guid in _effectGUIDProvider.Values)
            {
                KillEffect(guid);
            }
        }

        public void KillEffect(Volume volume, Player player)
        {
            TriggerEffectParameters effectParams = BuildEffect(volume);

            effectParams.SetRelevantPlayer(player.GetTransportConnection());

            KillEffect(effectParams.asset.GUID, player);
        }

        public void KillEffect(Volume volume)
        {
            TriggerEffectParameters effectParams = BuildEffect(volume);

            KillEffect(effectParams.asset.GUID);
        }

        private TriggerEffectParameters BuildEffect(Volume volume)
        {
            string effectName = $"{volume.Shape}_{volume.Color}_{volume.Material}";

            if (!_effectGUIDProvider.TryGetValue(effectName, out Guid guid))
            {
                throw new KeyNotFoundException($"The following key does not correspond to an existing volume: {effectName}");
            }

            return new TriggerEffectParameters(guid)
            {
                position = volume.Center,
                scale = volume.Size,
                direction = volume.Rotation
            };
        }

        private string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }

        private void ShowEffect(TriggerEffectParameters effectParameters, bool unique)
        {
            _threadAdapter.RunOnMainThread(() =>
            {
                if (unique)
                    EffectManager.ClearEffectByGuid_AllPlayers(effectParameters.asset.GUID);

                EffectManager.triggerEffect(effectParameters);
            });
        }

        private void KillEffect(Guid guid)
        {
            _threadAdapter.RunOnMainThread(() =>
            {
                EffectManager.ClearEffectByGuid_AllPlayers(guid);
            });
        }

        private void KillEffect(Guid guid, Player player)
        {
            _threadAdapter.RunOnMainThread(() =>
            {
                EffectManager.ClearEffectByGuid(guid, player.GetTransportConnection());
            });
        }
    }
}