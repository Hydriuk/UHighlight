#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    internal interface IVolumeTester : IDisposable
    {
        void StartTest(Player player, string category, string name);
        void StopTest(Player player);
    }
}