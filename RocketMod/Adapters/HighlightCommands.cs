﻿using Rocket.Core;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Linq;
using System.Threading.Tasks;
using UHighlight.API;

namespace UHighlight.RocketMod.Adapters
{
    public class HighlightCommands : IHighlightCommands
    {
        public Task ExecuteCancel(Player player)
        {
            Execute(player, new[]
            {
                "cancel"
            });

            return Task.CompletedTask;
        }

        public Task ExecuteCreate(Player player, string shape, string material, string color)
        {
            Execute(player, new[]
            {
                "create",
                shape, material, color
            });

            return Task.CompletedTask;
        }

        public Task ExecuteDelete(Player player, string group, string zone)
        {
            Execute(player, new[]
            {
                "delete",
                group, zone
            });

            return Task.CompletedTask;
        }

        public Task ExecuteGroups(Player player)
        {
            Execute(player, new[]
            {
                "listgroups"
            });

            return Task.CompletedTask;
        }

        public Task ExecuteShow(Player player, string group, string zone, float customSize = -1)
        {
            Execute(player, new[]
            {
                "show",
                group, zone, customSize.ToString()
            });

            return Task.CompletedTask;
        }

        public Task ExecuteValidate(Player player, string group, string zone)
        {
            Execute(player, new[]
            {
                "validate",
                group, zone
            });

            return Task.CompletedTask;
        }

        public Task ExecuteVolumes(Player player, string group)
        {
            Execute(player, new[]
            {
                "listvolumes",
                group
            });

            return Task.CompletedTask;
        }

        private void Execute(Player player, params string[] args) => Execute(UnturnedPlayer.FromPlayer(player), args.Prepend("uhighlight").ToArray());

        private void Execute(UnturnedPlayer uPlayer, params string[] args)
        {
            bool success = R.Commands.Execute(uPlayer, string.Join(" ", args));

            if (!success)
                throw new Exception("Error while executing command");
        }
    }
}