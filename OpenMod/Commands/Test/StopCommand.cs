using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using UHighlight.API;

namespace UHighlight.OpenMod.Commands.Test
{
    [CommandParent(typeof(TestCommand))]
    [Command("stop")]
    internal class StopCommand : UnturnedCommand
    {
        private readonly IVolumeTester _volumeTester;

        public StopCommand(IServiceProvider serviceProvider, IVolumeTester volumeTester) : base(serviceProvider)
        {
            _volumeTester = volumeTester;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            _volumeTester.StopTest(user.Player.Player);

            return UniTask.CompletedTask;
        }
    }
}