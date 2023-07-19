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
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

        public EffectBuilder(IThreadAdapter threadAdapter) 
        {
            _threadAdapter = threadAdapter;

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
            shape = ToWordCase(shape);
            color = ToWordCase(color);
            material = ToWordCase(material);
            
            return _effectGUIDProvider.ContainsKey($"{shape}_{material}_{color}");
        }

        public void DisplayEffect(Volume volume, bool unique = false)
        {
            TriggerEffectParameters effectParams = BuildEffect(volume);

            ShowEffect(effectParams, unique);
        }

        public void DisplayEffect(Volume volume, Player player, bool unique = false)
        {
            TriggerEffectParameters effectParams = BuildEffect(volume);

            effectParams.SetRelevantPlayer(player.GetTransportConnection());

            ShowEffect(effectParams, unique);
        }

        public void KillEffect(Volume volume, Player player)
        {
            TriggerEffectParameters effectParams = BuildEffect(volume);

            effectParams.SetRelevantPlayer(player.GetTransportConnection());

            KillEffect(effectParams);
        }

        public void KillEffect(Volume volume)
        {
            TriggerEffectParameters effectParams = BuildEffect(volume);

            KillEffect(effectParams);
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

        private string ToWordCase(string word)
        {
            return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
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

        private void KillEffect(TriggerEffectParameters effectParameters)
        {
            _threadAdapter.RunOnMainThread(() =>
            {
                EffectManager.ClearEffectByGuid_AllPlayers(effectParameters.asset.GUID);
            });
        }
    }
}
