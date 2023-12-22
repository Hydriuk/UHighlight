using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace UHighlight.RocketMod.Commands.Test
{
    public static class StartCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length != 2)
            {
                ChatManager.serverSendMessage("Wrong syntax : /hl test <group> <name>", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            UHighlightPlugin.Instance.VolumeTester.StartTest(uPlayer.Player, command[0], command[1]);
        }
    }
}