#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Threading.Tasks;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    public interface IVolumeTester : IDisposable
    {
        Task StartTest(Player player, string group, string name);
        Task StopTest(Player player);
    }
}