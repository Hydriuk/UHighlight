using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;

namespace UHighlight.OpenMod.Commands.Test
{
    [CommandParent(typeof(HighlightCommand))]
    [Command("test")]
    public class TestCommand : UnturnedCommand
    {
        public TestCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override UniTask OnExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}