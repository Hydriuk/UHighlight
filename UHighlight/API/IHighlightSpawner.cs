#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Collections.Generic;
using System.Threading.Tasks;
using UHighlight.Components;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    public interface IHighlightSpawner
    {
        IEnumerable<HighlightedZone> BuildZones(string group, float customSize = -1);
        HighlightedZone BuildZone(string group, string name, float customSize = -1);
    }
}