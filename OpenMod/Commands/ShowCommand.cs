using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHighlight.OpenMod.Commands
{
    [CommandParent(typeof(HighlightCommand))]
    [Command("show")]
    [CommandAlias("s")]
    [CommandActor(typeof(UnturnedUser))]
    public class ShowCommand : UnturnedCommand
    {
        public ShowCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            throw new NotImplementedException();
        }
    }
}
