using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    internal static class GroupsCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            IEnumerable<string> groups = UHighlightPlugin.Instance.VolumeStore.GetGroups();

            StringBuilder sb = new StringBuilder($"Volumes groups : ");

            foreach (string group in groups)
            {
                sb.Append(group);
                sb.Append(", ");
            }

            ChatManager.serverSendMessage(sb.ToString(), Color.green, toPlayer: uPlayer.SteamPlayer());
        }
    }
}