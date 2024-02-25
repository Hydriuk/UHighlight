#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Collections.Generic;
using UHighlight.Components;
using UHighlight.Models;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    public interface IHighlightBuilder
    {
        IEnumerable<HighlightedZone> BuildZones(string group, float customSize = -1);
        HighlightedZone BuildZone(string group, string name, float customSize = -1);
        HighlightedZone BuildZone(Volume volume, float customSize = -1);
    }
}