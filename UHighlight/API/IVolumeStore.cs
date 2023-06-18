#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Text;
using UHighlight.Models;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    public interface IVolumeStore : IDisposable
    {
        void Upsert(Volume volume);
        IEnumerable<string> GetCategories();
        IEnumerable<Volume> GetVolumes(string category);
        Volume GetVolume(string category, string name);
    }
}
