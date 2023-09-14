using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;

namespace UHighlight.OpenMod.Commands
{
    [Command("uhighlight")]
    [CommandAlias("uhl")]
    public class HighlightCommand : UnturnedCommand
    {
        public HighlightCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override UniTask OnExecuteAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}