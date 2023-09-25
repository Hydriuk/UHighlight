using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    internal static class CancelCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            UHighlightPlugin.Instance.VolumeEditor.StopEditing(uPlayer.Player);

            ChatManager.serverSendMessage("Volume editing canceled", Color.green, toPlayer: uPlayer.SteamPlayer());
        }
    }
}