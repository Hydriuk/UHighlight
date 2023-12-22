#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using UHighlight.API;
using UHighlight.EditionStrategies;
using UHighlight.Models;

namespace UHighlight.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    internal class VolumeEditor : IVolumeEditor
    {
        private readonly Dictionary<Player, IEditionStrategy> _editedVolumes = new Dictionary<Player, IEditionStrategy>();

        private readonly IChatAdapter _chatAdapter;
        private readonly IEffectBuilder _effectBuilder;
        private readonly IVolumeStore _volumeStore;

        public VolumeEditor(IChatAdapter chatAdapter, IEffectBuilder effectBuilder, IVolumeStore volumeStore)
        {
            _chatAdapter = chatAdapter;
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

            _chatAdapter.Send(player, $"Started editing a {material} {color} {shape}");
        }

        public void StopEditing(Player player)
        {
            if (!_editedVolumes.TryGetValue(player, out IEditionStrategy edition))
                return;

            edition.Cancel();

            _editedVolumes.Remove(player);

            _chatAdapter.Send(player, $"Editing stopped");
        }

        public void Validate(Player player, string group, string name)
        {
            if (!_editedVolumes.TryGetValue(player, out IEditionStrategy edition))
                return;

            Volume? volume = edition.Build();

            if (volume == null)
                throw new NullReferenceException("The volume is missing a datum to be generated");

            volume.Group = group;
            volume.Name = name;

            _editedVolumes.Remove(player);

            _volumeStore.Upsert(volume);

            _chatAdapter.Send(player, $"Volume {group} {name} created");
        }

        public void SetSize(Player player, float size)
        {
            if (!_editedVolumes.TryGetValue(player, out IEditionStrategy edition))
                return;

            edition.SetSize(size);

            _chatAdapter.Send(player, $"Size set to {size}");
        }
    }
}