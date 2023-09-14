#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using UHighlight.Models;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    internal interface IVolumeEditor : IDisposable
    {
        void StartEditing(Player player, EVolumeShape shape, string material, string color);
        void StopEditing(Player player);
        void Validate(Player player, string category, string name);
    }
}