using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using UHighlight.API;

namespace UHighlight.OpenMod.Commands
{
    [CommandParent(typeof(HighlightCommand))]
    [Command("delete")]
    [CommandSyntax("<category> <name>")]
    [CommandActor(typeof(UnturnedUser))]
    internal class DeleteCommand : UnturnedCommand
    {
        private readonly IVolumeStore _volumeStore;

        public DeleteCommand(IServiceProvider serviceProvider, IVolumeStore volumeStore) : base(serviceProvider)
        {
            _volumeStore = volumeStore;
        }

        protected override UniTask OnExecuteAsync()
        {
            if (Context.Parameters.Count != 2)
                throw new CommandWrongUsageException(Context);

            if (!_volumeStore.Exists(Context.Parameters[0], Context.Parameters[1]))
                throw new UserFriendlyException($"Volume {Context.Parameters[0]} {Context.Parameters[1]} does not exist");

            _volumeStore.DeleteVolume(Context.Parameters[0], Context.Parameters[1]);

            return UniTask.CompletedTask;
        }
    }
}