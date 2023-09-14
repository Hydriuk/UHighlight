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
    public interface IHighlightSpawner
    {
        IEnumerable<HighlightedZone> BuildZones(string category);
        HighlightedZone BuildZone(string category, string name);
    }
}