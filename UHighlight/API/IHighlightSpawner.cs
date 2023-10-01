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
        Task<IEnumerable<HighlightedZone>> BuildZones(string category, float customSize = -1);
        Task<HighlightedZone> BuildZone(string category, string name, float customSize = -1);
    }
}