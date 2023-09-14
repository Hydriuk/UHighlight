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
    internal interface IEffectBuilder : IDisposable
    {
        bool Exists(string shape, string color, string material);
        void DisplayEffect(Volume volume, bool unique = false);
        void DisplayEffect(Volume volume, Player player, bool unique = false);
        void KillEffect(Volume volume, Player player);
        void KillEffect(Volume volume);
    }
}