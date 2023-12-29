using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    internal class UICommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            UHighlightPlugin.Instance.AdminUIManager.ShowUI(uPlayer.Player);
        }
    }
}
