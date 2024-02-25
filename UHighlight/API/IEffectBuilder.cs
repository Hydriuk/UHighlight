#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using UHighlight.Models;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    public interface IEffectBuilder : IDisposable
    {
        bool Exists(string shape, string material, string color);
        void DisplayGroupEffects(string group, bool unique = false);
        void DisplayGroupEffects(string group, Player player, bool unique = false);
        void DisplayEffect(Volume volume, bool unique = false);
        void DisplayEffect(Volume volume, Player player, bool unique = false, float customSize = -1);
        void DisplayEffect(Volume volume, IEnumerable<Player> players, bool unique = false);
        void KillEffect(Volume volume, IEnumerable<Player> players);
        void KillEffect(Volume volume, Player player);
        void KillEffect(Volume volume);
        void KillAllEffects(Player player);
        void KillAllEffects();
    }
}