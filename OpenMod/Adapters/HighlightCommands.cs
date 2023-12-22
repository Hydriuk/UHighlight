using Hydriuk.UnturnedModules.Extensions;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using SDG.Unturned;
using System;
using System.Linq;
using System.Threading.Tasks;
using UHighlight.API;

namespace UHighlight.OpenMod.Adapters
{
    [ServiceImplementation]
    public class HighlightCommands : IHighlightCommands
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IUserManager _userManager;

        public HighlightCommands(IOpenModComponent openmodComponent, ICommandExecutor commandExecutor, IUserManager userManager, IEventBus eventBus)
        {
            _commandExecutor = commandExecutor;
            _userManager = userManager;
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

        public Task ExecuteShow(Player player, string group, string zone, float customSize = -1) => Execute(player, new[]
        {
            "show", group, zone, customSize.ToString()
        });

        public Task ExecuteGroups(Player player) => Execute(player, new[]
        {
            "groups"
        });

        public Task ExecuteVolumes(Player player, string group) => Execute(player, new[]
{
            "volumes", group
        });

        private async Task Execute(Player player, params string[] args) => await Execute(await GetUser(player), args.Prepend("uhighlight").ToArray());

        private Task Execute(ICommandActor actor, params string[] args) => _commandExecutor.ExecuteAsync(actor, args, "/");

        private async Task<IUser> GetUser(Player player)
        {
            IUser? user = await _userManager.FindUserAsync(KnownActorTypes.Player, player.GetSteamID().ToString(), UserSearchMode.FindById);

            if (user == null)
                throw new Exception($"User {player.GetSteamID()} not found");

            return user;
        }
    }
}