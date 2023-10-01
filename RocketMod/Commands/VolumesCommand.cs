using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Text;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    internal static class VolumesCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length != 1)
            {
                ChatManager.serverSendMessage("Wrong syntax : /uhl volumes <category>", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            IEnumerable<Volume> volumes = UHighlightPlugin.Instance.VolumeStore.GetVolumes(command[0]);

            StringBuilder sb = new StringBuilder($"{command[0]}'s volumes : ");

            foreach (Volume volume in volumes)
            {
                sb.Append(volume.Name);
                sb.Append(", ");
            }

            ChatManager.serverSendMessage(sb.ToString(), Color.green, toPlayer: uPlayer.SteamPlayer());
        }
    }
}