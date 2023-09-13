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
    public static class DeleteCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length != 2)
            {
                ChatManager.serverSendMessage("Wrong syntax : /hl delete <category> <name>", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            if (!UHighlightPlugin.Instance.VolumeStore.Exists(command[0], command[1]))
            {
                ChatManager.serverSendMessage($"Volume {command[0]} {command[1]} does not exist", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            UHighlightPlugin.Instance.VolumeStore.DeleteVolume(command[0], command[1]);
        }
    }
}
