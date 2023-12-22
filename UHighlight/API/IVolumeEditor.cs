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
    public interface IVolumeEditor : IDisposable
    {
        bool IsEditing(Player player);
        void StartEditing(Player player, EVolumeShape shape, string material, string color);
        void StopEditing(Player player);
        void Validate(Player player, string group, string name);
        void SetSize(Player player, float size);
    }
}