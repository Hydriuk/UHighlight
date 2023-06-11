using Rocket.API;
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
    public class HighlightCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "highlight";

        public string Help => "Help";

        public string Syntax => "";

        public List<string> Aliases => new List<string>() { "hl" };

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller; 

            if(command.Length == 0)
            {
                ChatManager.serverSendMessage("Not enough params", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            if (command[0] == "create" || command[0] == "c")
            {
                CreateCommand.Execute(uPlayer, command.Skip(1).ToArray());
            }
            else if(command[0] == "show" || command[0] == "s")
            {

            }
        }
    }
}
