#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UHighlight.API;
using UHighlight.Components;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    internal class HighlightBuilder : IHighlightBuilder
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        private readonly IVolumeStore _volumeStore;
        private readonly IEffectBuilder _effectBuilder;

        public HighlightBuilder(IVolumeStore volumeStore, IEffectBuilder effectBuilder)
        {
            _volumeStore = volumeStore;
            _effectBuilder = effectBuilder;
        }

        public async Task<IEnumerable<HighlightedZone>> BuildZones(string group, float customSize = -1)
        {
            IEnumerable<Volume> volumes = _volumeStore.GetVolumes(group);

            List<HighlightedZone> zones = new List<HighlightedZone>();
            foreach (var volume in volumes)
            {
                zones.Add(await BuildZone(volume, customSize));
            }

            return zones;
        }

        public async Task<HighlightedZone> BuildZone(string group, string name, float customSize = -1)
        {
            Volume volume = _volumeStore.GetVolume(group, name);

            return await BuildZone(volume, customSize);
        }

        public async Task<HighlightedZone> BuildZone(Volume volume, float customSize = -1)
        {
            await _lock.WaitAsync();

            GameObject go = new GameObject();

            go.transform.position = volume.Center;
            go.transform.localScale = customSize == -1 ? volume.Size : Vector3.one * customSize;
            go.transform.rotation = Quaternion.FromToRotation(Vector3.forward, volume.Rotation);
            go.layer = LayerMasks.TRAP;

            Collider collider = volume.Shape switch
            {
                EVolumeShape.Cube => go.AddComponent<BoxCollider>(),
                EVolumeShape.Sphere => go.AddComponent<SphereCollider>(),
                //"Cylinder" => go.AddComponent<CapsuleCollider>(),
                _ => throw new Exception()
            };

            collider.isTrigger = true;

            HighlightedZone zone = go.AddComponent<HighlightedZone>();

            zone.Init(_effectBuilder, volume.Group, volume.Name, volume);

            _lock.Release();

            return zone;
        }
    }
}