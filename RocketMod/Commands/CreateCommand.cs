using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    internal class CreateCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length != 3)
            {
                ChatManager.serverSendMessage("Not enough params", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            if (UHighlightPlugin.Instance.VolumeEditor.IsEditing(uPlayer.Player))
            {
                ChatManager.serverSendMessage("You are already editing a zone", Color.red, toPlayer: uPlayer.SteamPlayer(), useRichTextFormatting: true);
                return;
            }

            if (!UHighlightPlugin.Instance.EffectBuilder.Exists(command[0], command[1], command[2]))
            {
                ChatManager.serverSendMessage($"Shape <b>{command[0]}</b> with material <b>{command[1]}</b> and color <b>{command[2]}</b> is not known", Color.red, toPlayer: uPlayer.SteamPlayer(), useRichTextFormatting: true);
                return;
            }

            UHighlightPlugin.Instance.VolumeEditor.StartEditing(uPlayer.Player, command[0], command[1], command[2]);
        }
    }
}