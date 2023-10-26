using HarmonyLib;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UHighlight.Patches
{
    [HarmonyPatch
    (
        typeof(BarricadeManager), 
        nameof(BarricadeManager.destroyBarricade),
        typeof(BarricadeDrop),
        typeof(byte),
        typeof(byte),
        typeof(ushort)
    )]
    internal class BarricadeDestroyedPatch
    {
        public static event Action<BarricadeDrop>? BarricadeDestroyed;

        private static void Postfix(BarricadeDrop barricade)
        {
            BarricadeDestroyed?.Invoke(barricade);
        }
    }
}
