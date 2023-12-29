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
    internal interface IAdminUIManager : IDisposable
    {
        void ShowUI(Player player);
    }
}
