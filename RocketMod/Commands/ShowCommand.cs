﻿using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    public static class ShowCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            if (command.Length != 2)
            {
                ChatManager.serverSendMessage("Wrong syntax : /hl show <category> <name>", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            Volume volume = Plugin.Instance.VolumeStore.GetVolume(command[0], command[1]);

            if (volume == null)
            {
                ChatManager.serverSendMessage($"Volume {command[1]} was not found in category {command[0]}", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            Plugin.Instance.EffectBuilder.DisplayEffect(volume, uPlayer.Player, true);
        }
    }
}