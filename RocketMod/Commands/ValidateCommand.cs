using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    public static class ValidateCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length != 2)
            {
                ChatManager.serverSendMessage("Wrong syntax : /hl validate <category> <name>", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            UHighlightPlugin.Instance.VolumeEditor.Validate(uPlayer.Player, command[0], command[1]);
        }
    }
}