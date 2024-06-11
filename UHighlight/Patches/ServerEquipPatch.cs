using HarmonyLib;
using Hydriuk.UnturnedModules.Extensions;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;

namespace UHighlight.Patches
{
    [HarmonyPatch
    (
        typeof(PlayerEquipment),
        nameof(PlayerEquipment.ServerEquip)
    )]
    internal class ServerEquipPatch
    {
        /// <summary>
        /// Return false to cancel
        /// </summary>
        public static event Func<Player, Item, bool>? PlayerEquip;

        private static bool Prefix(PlayerEquipment __instance, byte page, byte x, byte y)
        {
            Player player = __instance.player;

            Item? item = player.inventory.GetItem(page, x, y);

            if (item == null)
                return true;

            return PlayerEquip?.Invoke(player, item) ?? true;
        }
    }
}
