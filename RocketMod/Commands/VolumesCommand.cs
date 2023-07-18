using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    public static class VolumesCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length != 1)
            {
                ChatManager.serverSendMessage("Wrong syntax : /hl volumes <category>", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            IEnumerable<Volume> volumes = Plugin.Instance.VolumeStore.GetVolumes(command[0]);

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
