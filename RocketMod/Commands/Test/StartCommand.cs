using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UHighlight.RocketMod.Commands.Test
{
    public static class StartCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {

            if (command.Length != 2)
            {
                ChatManager.serverSendMessage("Wrong syntax : /hl test <category> <name>", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            UHighlightPlugin.Instance.VolumeTester.StartTest(uPlayer.Player, command[0], command[1]);
        }
    }
}
