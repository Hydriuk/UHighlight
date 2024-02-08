#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    public interface IZonePropertyController : IDisposable
    {
        void Refresh();
    }
}
