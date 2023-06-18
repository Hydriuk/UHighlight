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

namespace UHighlight.OpenMod.Commands.Test
{
    [CommandParent(typeof(TestCommand))]
    [Command("start")]
    [CommandSyntax("<category> <name>")]
    public class StartCommand : UnturnedCommand
    {
        private readonly IVolumeTester _volumeTester;

        public StartCommand(IServiceProvider serviceProvider, IVolumeTester volumeTester) : base(serviceProvider)
        {
            _volumeTester = volumeTester;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            if (Context.Parameters.Count != 2)
                throw new CommandWrongUsageException(Context);

            _volumeTester.StartTest(user.Player.Player, Context.Parameters[0], Context.Parameters[1]);

            return UniTask.CompletedTask;
        }
    }
}
