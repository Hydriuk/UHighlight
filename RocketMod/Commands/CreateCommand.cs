using Rocket.Unturned.Player;
using RocketMod;
using SDG.Unturned;
using System;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    public static class CreateCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length != 3)
            {
                ChatManager.serverSendMessage("Not enough params", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            if (!Enum.TryParse(command[0], out EVolumeShape shape))
            {
                ChatManager.serverSendMessage($"<b>{shape}</b> was not recognized. Available shapes : {EVolumeShape.Cube}, {EVolumeShape.Cylinder}, {EVolumeShape.Sphere}", Color.red, toPlayer: uPlayer.SteamPlayer(), useRichTextFormatting: true);
                return;
            }

            if (!Plugin.Instance.EffectBuilder.Exists(command[0], command[1], command[2]))
            {
                ChatManager.serverSendMessage($"Shape <b>{command[0]}</b> with material <b>{command[1]}</b> and color <b>{command[2]}</b> is not known", Color.red, toPlayer: uPlayer.SteamPlayer(), useRichTextFormatting: true);
                return;
            }

            Plugin.Instance.VolumeEditor.StartEditing(uPlayer.Player, shape, command[1], command[2]);
        }
    }
}
