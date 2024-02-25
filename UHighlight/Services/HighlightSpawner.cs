#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Collections.Generic;
using UHighlight.API;
using UHighlight.Components;
using UHighlight.Models;

namespace UHighlight.Services
{
#if OPENMOD
    [ServiceImplementation]
#endif
    internal class HighlightSpawner : IHighlightSpawner
    {
        private readonly IHighlightBuilder _highlightBuilder;

        public HighlightSpawner(IHighlightBuilder highlightBuilder)
        {
            _highlightBuilder = highlightBuilder;
        }

        public IEnumerable<HighlightedZone> BuildZones(string group, float customSize = -1)
        {
            return _highlightBuilder.BuildZones(group, customSize);
        }

        public HighlightedZone BuildZone(string group, string name, float customSize = -1)
        {
            return _highlightBuilder.BuildZone(group, name, customSize);
        }

        public HighlightedZone BuildZone(Volume volume)
        {
            return _highlightBuilder.BuildZone(volume);
        }
    }
}