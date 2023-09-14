using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using UHighlight.RocketMod.Commands.Test;
using UnityEngine;

namespace UHighlight.RocketMod.Commands
{
    public class HighlightCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "uhighlight";

        public string Help => "Help";

        public string Syntax => "";

        public List<string> Aliases => new List<string>() { "uhl" };

        public List<string> Permissions => new List<string>() { "uhighlight.admin" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;

            if (command.Length == 0)
            {
                ChatManager.serverSendMessage("Not enough params", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            string[] subCommand = command.Skip(1).ToArray();

            switch (command[0])
            {
                case "create":
                case "c":
                    CreateCommand.Execute(uPlayer, subCommand);
                    break;

                case "categories":
                case "cat":
                    CategoriesCommand.Execute(uPlayer, subCommand);
                    break;

                case "show":
                case "s":
                    ShowCommand.Execute(uPlayer, subCommand);
                    break;

                case "validate":
                case "v":
                    ValidateCommand.Execute(uPlayer, subCommand);
                    break;

                case "volumes":
                case "vol":
                    VolumesCommand.Execute(uPlayer, subCommand);
                    break;

                case "delete":
                    DeleteCommand.Execute(uPlayer, subCommand);
                    break;

                case "cancel":
                    CancelCommand.Execute(uPlayer, subCommand);
                    break;

                case "test":
                    if (command[1] == "start")
                        StartCommand.Execute(uPlayer, subCommand.Skip(1).ToArray());
                    else if (command[1] == "stop")
                        StopCommand.Execute(uPlayer, subCommand.Skip(1).ToArray());

                    break;

                default:
                    ChatManager.serverSendMessage("Wrong syntax", Color.red, toPlayer: uPlayer.SteamPlayer());
                    return;
            }
        }
    }
}