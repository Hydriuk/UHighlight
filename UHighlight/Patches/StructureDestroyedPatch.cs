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
        typeof(StructureManager), 
        nameof(StructureManager.destroyStructure),
        typeof(StructureDrop),
        typeof(byte),
        typeof(byte),
        typeof(Vector3),
        typeof(bool)
    )]
    internal class StructureDestroyedPatch
    {
        public static event Action<StructureDrop>? StructureDestroyed;

        private static void Postfix(StructureDrop structure)
        {
            StructureDestroyed?.Invoke(structure);
        }
    }
}
