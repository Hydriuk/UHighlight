﻿#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Text;
using UHighlight.Components;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    internal interface IHighlightBuilder
    {
        IEnumerable<HighlightedZone> BuildZones(string category);
        HighlightedZone BuildZone(string category, string name);
    }
}
