using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    public static class CancelCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            UHighlightPlugin.Instance.VolumeEditor.StopEditing(uPlayer.Player);

            ChatManager.serverSendMessage("Volume editing canceled", Color.green, toPlayer: uPlayer.SteamPlayer());
        }
    }
}
