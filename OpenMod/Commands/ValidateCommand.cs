using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using UHighlight.API;

namespace UHighlight.OpenMod.Commands
{
    [CommandParent(typeof(HighlightCommand))]
    [Command("validate")]
    [CommandAlias("v")]
    [CommandSyntax("<group> <name>")]
    [CommandActor(typeof(UnturnedUser))]
    internal class ValidateCommand : UnturnedCommand
    {
        private readonly IVolumeEditor _volumeEditor;

        public ValidateCommand(IServiceProvider serviceProvider, IVolumeEditor volumeEditor) : base(serviceProvider)
        {
            _volumeEditor = volumeEditor;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            if (!_volumeEditor.IsEditing(user.Player.Player))
                throw new CommandWrongUsageException("You are not currently editing a zone");

            if (Context.Parameters.Count != 2)
                throw new CommandWrongUsageException(Context);

            _volumeEditor.Validate(user.Player.Player, Context.Parameters[0], Context.Parameters[1]);

            return UniTask.CompletedTask;
        }
    }
}