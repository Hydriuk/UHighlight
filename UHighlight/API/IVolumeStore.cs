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
        bool Exists(string group, string name);
        void Upsert(Volume volume);
        IEnumerable<string> GetGroups();
        IEnumerable<Volume> GetVolumes(string group);
        Volume GetVolume(string group, string name);
        void DeleteVolume(string group, string name);
    }
}