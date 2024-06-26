﻿using Rocket.Unturned.Player;
using SDG.Unturned;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    internal static class ShowCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length < 2)
            {
                ChatManager.serverSendMessage("Wrong syntax : /uhl show <group> <name>", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            Volume volume = UHighlightPlugin.Instance.VolumeStore.GetVolume(command[0], command[1]);

            if (volume == null)
            {
                ChatManager.serverSendMessage($"Volume {command[1]} was not found in group {command[0]}", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            if (command.Length > 2 && float.TryParse(command[2], out float customSize))
            {
                volume.Size = Vector3.one * customSize;

                UHighlightPlugin.Instance.EffectBuilder.DisplayEffect(volume, uPlayer.Player, true, customSize);
            }
            else
            {
                UHighlightPlugin.Instance.EffectBuilder.DisplayEffect(volume, uPlayer.Player, true);
            }
        }
    }
}