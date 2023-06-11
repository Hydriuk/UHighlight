﻿#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using UHighlight.Models;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    public interface IEffectBuilder : IDisposable
    {
        bool Exists(string shape, string color, string material);
        void DisplayEffect(Volume volume, bool unique = false);
        void DisplayEffect(Volume volume, Player player, bool unique = false);
    }
}
