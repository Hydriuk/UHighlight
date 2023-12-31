#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using UHighlight.Models;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    public interface IVolumeStore : IDisposable
    {
        bool Exists(string groupName, string zoneName);
        void Upsert(Volume volume);
        void CreateGroup(string groupName);
        IEnumerable<string> GetGroupNames();
        IEnumerable<ZoneGroup> GetGroups();
        ZoneGroup GetGroup(string groupName);
        IEnumerable<Volume> GetVolumes(string groupName);
        Volume GetVolume(string groupName, string zoneName);
        void DeleteVolume(string groupName, string zoneName);
        void DeleteGroup(string groupName);
    }
}