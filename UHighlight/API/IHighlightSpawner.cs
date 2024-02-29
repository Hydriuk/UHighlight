#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UHighlight.Components;
using UHighlight.Models;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    public interface IHighlightSpawner : IDisposable
    {
        /// <summary>
        /// Spawns all zones from the group
        /// </summary>
        /// <param name="group">Name of the group to spawn the zones of</param>
        /// <param name="customSize">Override the size of the zone</param>
        /// <returns>The list of spawned zones</returns>
        Task<IEnumerable<HighlightedZone>> BuildZones(string group, float customSize = -1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group">Name of the group of the zone to spawn</param>
        /// <param name="name">Name of the zone to spawn</param>
        /// <param name="customSize">Override the size of the zone</param>
        /// <returns>The spawned zone</returns>
        Task<HighlightedZone> BuildZone(string group, string name, float customSize = -1);

        /// <summary>
        /// Spawn a zone from a <see cref="Volume"/>'s data 
        /// </summary>
        /// <param name="volume"></param>
        /// <returns>The spawned zone</returns>
        Task<HighlightedZone> BuildZone(Volume volume);
    }
}