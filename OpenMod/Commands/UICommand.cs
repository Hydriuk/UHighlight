using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHighlight.API;

namespace UHighlight.OpenMod.Commands
{
    [CommandParent(typeof(HighlightCommand))]
    [Command("ui")]
    internal class UICommand : UnturnedCommand
    {
        private readonly IAdminUIManager _adminUIManager;

        public UICommand(IServiceProvider serviceProvider, IAdminUIManager adminUIManager) : base(serviceProvider)
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
