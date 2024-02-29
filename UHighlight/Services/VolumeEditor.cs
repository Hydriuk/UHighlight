#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using UHighlight.API;
using UHighlight.EditionStrategies;
using UHighlight.Extensions;
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

        public void StartEditing(Player player, string shapeString, string materialString, string colorString)
        {
            if(
                !shapeString.TryParseVolumeShape(out EVolumeShape shape) ||
                !materialString.TryParseVolumeMaterial(out EVolumeMaterial material) ||
                !colorString.TryParseVolumeColor(out EVolumeColor color)
            )
                return;

            StartEditing(player, shape, material, color);
        }

        public void StartEditing(Player player, EVolumeShape shape, EVolumeMaterial material, EVolumeColor color)
        {
            if(_editedVolumes.ContainsKey(player))
            {
                _chatAdapter.Send(player, $"You are already editing a volume");
                return;
            }

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
            {
                _chatAdapter.Send(player, $"You are not editing a volume");
                return;
            }

            edition.Cancel();

            _editedVolumes.Remove(player);

            _chatAdapter.Send(player, $"Editing stopped");
        }

        public void Validate(Player player, string group, string name)
        {
            if(string.IsNullOrWhiteSpace(group))
            {
                _chatAdapter.SendError(player, "You must enter a group name for the volume");
                return;
            }

            if (!_volumeStore.Exists(group))
            {
                _chatAdapter.SendError(player, $"Group {group} not found");
                return;
            }

            if(string.IsNullOrWhiteSpace(name))
            {
                _chatAdapter.SendError(player, "You must enter a name for the volume");
                return;
            }

            if (!_editedVolumes.TryGetValue(player, out IEditionStrategy edition))
                return;

            Volume? volume = edition.Build();

            if (volume == null)
            {
                _chatAdapter.SendError(player, "The volume is not complete. Make sure every dimension of the volume is set.");
                return;
            }

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