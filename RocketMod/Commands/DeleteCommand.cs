using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    public static class DeleteCommand
    {
        internal static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length != 2)
            {
                ChatManager.serverSendMessage("Wrong syntax : /uhl delete <group> <name>", Color.red, toPlayer: uPlayer.SteamPlayer());
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