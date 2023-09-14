using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using UHighlight.API;
using UHighlight.Models;

namespace UHighlight.OpenMod.Commands
{
    [CommandParent(typeof(HighlightCommand))]
    [Command("show")]
    [CommandAlias("s")]
    [CommandSyntax("<category> <name>")]
    [CommandActor(typeof(UnturnedUser))]
    internal class ShowCommand : UnturnedCommand
    {
        private readonly IVolumeStore _volumeStore;
        private readonly IEffectBuilder _effectBuilder;

        public ShowCommand(IServiceProvider serviceProvider, IVolumeStore volumeStore, IEffectBuilder effectBuilder) : base(serviceProvider)
        {
            _volumeStore = volumeStore;
            _effectBuilder = effectBuilder;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            if (Context.Parameters.Count != 2)
                throw new CommandWrongUsageException(Context);

            Volume volume = _volumeStore.GetVolume(Context.Parameters[0], Context.Parameters[1]);

            if (volume == null)
                throw new UserFriendlyException($"Volume {Context.Parameters[1]} was not found in category {Context.Parameters[0]}");

            _effectBuilder.DisplayEffect(volume, user.Player.Player, true);

            return UniTask.CompletedTask;
        }
    }
}