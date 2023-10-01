﻿using Hydriuk.UnturnedModules.Adapters;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using UHighlight.API;
using UHighlight.Models;
using UHighlight.VolumeEditors;
using UHighlight.VolumeStrategies;

namespace UHighlight.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    internal class VolumeEditor : IVolumeEditor
    {
        private readonly Dictionary<Player, IEditionStrategy> _editedVolumes = new Dictionary<Player, IEditionStrategy>();

        private readonly ICoroutineAdapter _coroutineAdapter;
        private readonly IEffectBuilder _effectBuilder;
        private readonly IVolumeStore _volumeStore;

        public VolumeEditor(ICoroutineAdapter coroutineAdapter, IEffectBuilder effectBuilder, IVolumeStore volumeStore)
        {
            _coroutineAdapter = coroutineAdapter;
            _effectBuilder = effectBuilder;
            _volumeStore = volumeStore;
        }

        public void Dispose()
        {
            foreach (var editedVolume in _editedVolumes.Values)
            {
                editedVolume.Dispose();
            }
        }

        public bool IsEditing(Player player)
        {
            return _editedVolumes.ContainsKey(player);
        }

        public void StartEditing(Player player, EVolumeShape shape, string material, string color)
        {
            IEditionStrategy strategy = shape switch
            {
                EVolumeShape.Cube => new CubeStrategy(_effectBuilder, player, material, color),
                EVolumeShape.Sphere => new SphereStrategy(_effectBuilder, player, material, color),
                EVolumeShape.Cylinder => new CylinderStrategy(_effectBuilder, player, material, color),
                _ => throw new Exception("Unknown strategy")
            };

            _editedVolumes.Add(player, strategy);
        }

        public void StopEditing(Player player)
        {
            if (!_editedVolumes.TryGetValue(player, out IEditionStrategy edition))
                return;

            edition.Cancel();

            _editedVolumes.Remove(player);
        }

        public void Validate(Player player, string category, string name)
        {
            if (!_editedVolumes.TryGetValue(player, out IEditionStrategy edition))
                return;

            Volume? volume = edition.Build();

            if (volume == null)
                throw new NullReferenceException("The volume is missing a datum to be generated");

            volume.Category = category;
            volume.Name = name;

            _editedVolumes.Remove(player);

            _volumeStore.Upsert(volume);
        }

        public void SetSize(Player player, float size)
        {
            if (!_editedVolumes.TryGetValue(player, out IEditionStrategy edition))
                return;

            edition.SetSize(size);
        }
    }
}