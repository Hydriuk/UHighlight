#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Collections.Generic;
using UHighlight.Components;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    internal interface IHighlightBuilder
    {
        IEnumerable<HighlightedZone> BuildZones(string category, float customSize = -1);
        HighlightedZone BuildZone(string category, string name, float customSize = -1);
    }
}