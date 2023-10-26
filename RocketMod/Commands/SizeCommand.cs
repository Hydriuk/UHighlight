using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    internal class SizeCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length != 1)
            {
                ChatManager.serverSendMessage("Wrong syntax : /uhl size <size>", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            if (!UHighlightPlugin.Instance.VolumeEditor.IsEditing(uPlayer.Player))
            {
                ChatManager.serverSendMessage("You are not editing a zone", Color.red, toPlayer: uPlayer.SteamPlayer(), useRichTextFormatting: true);
                return;
            }

            if (!float.TryParse(command[0], out float size))
            {
                ChatManager.serverSendMessage("Wrong syntax : /uhl size <size>", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            UHighlightPlugin.Instance.VolumeEditor.SetSize(uPlayer.Player, size);
        }
    }
}
