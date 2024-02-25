#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IVolumeStore _volumeStore;
        private readonly IEffectBuilder _effectBuilder;

        public HighlightBuilder(IVolumeStore volumeStore, IEffectBuilder effectBuilder)
        {
            _volumeStore = volumeStore;
            _effectBuilder = effectBuilder;
        }

        public IEnumerable<HighlightedZone> BuildZones(string group, float customSize = -1)
        {
            return _volumeStore
                .GetVolumes(group)
                .Select(volume => BuildZone(volume, customSize))
                // Call ToList to prevent the method to be called more than once for a single call
                // Select contains a yield return, and these makes the IEnumerable to be executed each time it is enumerated
                .ToList();
        }

        public HighlightedZone BuildZone(string group, string name, float customSize = -1)
        {
            Volume volume = _volumeStore.GetVolume(group, name);

            return BuildZone(volume, customSize);
        }

        private HighlightedZone BuildZone(Volume volume, float customSize)
        {
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

            return zone;
        }
    }
}