using Cysharp.Threading.Tasks;
using Hydriuk.UnturnedModules.Extensions;
using OpenMod.API.Commands;
using OpenMod.API.Users;
using OpenMod.Core.Console;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHighlight.API;
using UnityEngine;

namespace UHighlight.OpenMod.Adapters
{
    public class HighlightAdapter : IHighlightAdapter
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly ICommandActor _consoleActor;
        private readonly IUserProvider _userProvider;

        public HighlightAdapter(ICommandExecutor commandExecutor, IConsoleActorAccessor consoleActorAccessor, IUserProvider userProvider) 
        {
            _commandExecutor = commandExecutor;
            _consoleActor = consoleActorAccessor.Actor;
            _userProvider = userProvider;
        }

        public Task ExecuteCreate(Player player, string shape, string material, string color) => Execute(player, new[] 
        { 
            "create", shape, material, color 
        });

        public Task ExecuteCancel(Player player) => Execute(player, new[]
        {
            "cancel"
        });

        public Task ExecuteValidate(Player player, string group, string zone) => Execute(player, new[]
        {
            "validate", group, zone
        });

        public Task ExecuteDelete(Player player, string group, string zone) => Execute(player, new[]
        {
            "delete", group, zone
        });

        public Task ExecuteShow(Player player, string group, string zone) => Execute(player, new[]
        {
            "show", group, zone
        });

        public Task ExecuteGroups(Player player) => Execute(player, new[]
        {
            "categories"
        });

        public Task ExecuteVolumes(Player player, string group) => Execute(player, new[]
{
            "volumes", group
        });

        private async Task Execute(Player player, params string[] args) => await Execute(await GetUser(player), args.Prepend("uhighlight").ToArray());
        private Task Execute(ICommandActor actor, params string[] args) => _commandExecutor.ExecuteAsync(actor, args, "/");

        private async Task<IUser> GetUser(Player player)
        {
            IUser? user = await _userProvider.FindUserAsync(KnownActorTypes.Player, player.GetSteamID().ToString(), UserSearchMode.FindById);

            if (user == null)
                throw new Exception($"User {player.GetSteamID()} not found");

            return user;
        }
    }
}
