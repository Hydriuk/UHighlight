using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using UHighlight.API;

namespace UHighlight.OpenMod.Commands
{
    [Command("uhighlight")]
    [CommandAlias("uhl")]
    internal class HighlightCommand : UnturnedCommand
    {
        private readonly IAdminUIManager _adminUIManager;

        public HighlightCommand(IServiceProvider serviceProvider, IAdminUIManager adminUIManager) : base(serviceProvider)
        {
            _adminUIManager = adminUIManager;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            _adminUIManager.ShowUI(user.Player.Player);

            return UniTask.CompletedTask;
        }
    }
}