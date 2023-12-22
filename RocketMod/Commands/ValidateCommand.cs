using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    internal static class ValidateCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length != 2)
            {
                ChatManager.serverSendMessage("Wrong syntax : /uhl validate <group> <name>", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            if (!UHighlightPlugin.Instance.VolumeEditor.IsEditing(uPlayer.Player))
            {
                ChatManager.serverSendMessage("You are already editing a zone", Color.red, toPlayer: uPlayer.SteamPlayer(), useRichTextFormatting: true);
                return;
            }

            UHighlightPlugin.Instance.VolumeEditor.Validate(uPlayer.Player, command[0], command[1]);
        }
    }
}