#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UHighlight.Components;
using UHighlight.Models;
using UnityEngine.UIElements;
using UnityEngine;
using UHighlight.API;

namespace UHighlight.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class HighlightBuilder : IHighlightBuilder
    {
        private readonly IVolumeStore _volumeStore;

        public HighlightBuilder(IVolumeStore volumeStore) 
        {
            _volumeStore = volumeStore;
        }

        public IEnumerable<HighlightedZone> BuildZones(string category)
        {
            IEnumerable<Volume> volumes = _volumeStore.GetVolumes(category);

            foreach (Volume volume in volumes)
            {
                yield return BuildZone(volume);
            }
        }

        public HighlightedZone BuildZone(string category, string name)
        {
            Volume volume = _volumeStore.GetVolume(category, name);

            return BuildZone(volume);
        }

        private HighlightedZone BuildZone(Volume volume)
        {
            GameObject go = new GameObject();

            go.transform.position = volume.Center;
            go.transform.localScale = volume.Size;
            go.transform.eulerAngles = volume.Rotation;
            go.layer = LayerMasks.TRAP;

            BoxCollider boxCollider = go.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;

            HighlightedZone zone = go.AddComponent<HighlightedZone>();
            zone.Init(volume.Category, volume.Name, volume);

            return zone;
        }
    }
}
