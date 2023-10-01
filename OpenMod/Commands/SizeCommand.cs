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
    [Command("size")]
    [CommandAlias("setsize")]
    [CommandSyntax("<size>")]
    [CommandActor(typeof(UnturnedUser))]
    internal class SizeCommand : UnturnedCommand
    {
        private readonly IVolumeEditor _volumeEditor;

        public SizeCommand(IServiceProvider serviceProvider, IVolumeEditor volumeEditor) : base(serviceProvider)
        {
            _volumeEditor = volumeEditor;
        }

        protected override async UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            if (!_volumeEditor.IsEditing(user.Player.Player))
                throw new CommandWrongUsageException("You are not currently editing a zone");

            if (Context.Parameters.Count != 1)
                throw new CommandWrongUsageException(Context);

            float size = await Context.Parameters.GetAsync<float>(0);

            _volumeEditor.SetSize(user.Player.Player, size);
        }
    }
}
