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
        bool Exists(string category, string name);
        void Upsert(Volume volume);
        IEnumerable<string> GetCategories();
        IEnumerable<Volume> GetVolumes(string category);
        Volume GetVolume(string category, string name);
        void DeleteVolume(string category, string name);
    }
}