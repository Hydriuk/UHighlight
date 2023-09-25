using Rocket.Unturned.Player;
using SDG.Unturned;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    internal static class ShowCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length != 2)
            {
                ChatManager.serverSendMessage("Wrong syntax : /hl show <category> <name>", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            Volume volume = UHighlightPlugin.Instance.VolumeStore.GetVolume(command[0], command[1]);

            if (volume == null)
            {
                ChatManager.serverSendMessage($"Volume {command[1]} was not found in category {command[0]}", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            UHighlightPlugin.Instance.EffectBuilder.DisplayEffect(volume, uPlayer.Player, true);
        }
    }
}