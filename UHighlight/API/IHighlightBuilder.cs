#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Collections.Generic;
using System.Threading.Tasks;
using UHighlight.Components;
using UHighlight.Models;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    public interface IHighlightBuilder
    {
        Task<IEnumerable<HighlightedZone>> BuildZones(string group, float customSize = -1);
        Task<HighlightedZone> BuildZone(string group, string name, float customSize = -1);
        Task<HighlightedZone> BuildZone(Volume volume, float customSize = -1);
    }
}