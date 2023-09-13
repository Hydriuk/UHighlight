using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    public static class CategoriesCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            IEnumerable<string> categories = UHighlightPlugin.Instance.VolumeStore.GetCategories();

            StringBuilder sb = new StringBuilder($"Volumes categories : ");

            foreach (string category in categories)
            {
                sb.Append(category);
                sb.Append(", ");
            }

            ChatManager.serverSendMessage(sb.ToString(), Color.green, toPlayer: uPlayer.SteamPlayer());
        }
    }
}
