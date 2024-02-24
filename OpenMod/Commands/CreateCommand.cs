using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using System.Globalization;
using UHighlight.API;
using UHighlight.Models;

namespace UHighlight.OpenMod.Commands
{
    [CommandParent(typeof(HighlightCommand))]
    [Command("create")]
    [CommandAlias("c")]
    [CommandSyntax("<shape> <material> <color>")]
    [CommandActor(typeof(UnturnedUser))]
    internal class CreateCommand : UnturnedCommand
    {
        private readonly IVolumeEditor _volumeEditor;
        private readonly IEffectBuilder _effectBuilder;

        public CreateCommand(IServiceProvider serviceProvider, IVolumeEditor volumeEditor, IEffectBuilder effectBuilder) : base(serviceProvider)
        {
            _volumeEditor = volumeEditor;
            _effectBuilder = effectBuilder;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            if (_volumeEditor.IsEditing(user.Player.Player))
                throw new CommandWrongUsageException("You are already editing a zone");

            if (Context.Parameters.Count != 3)
                throw new CommandWrongUsageException(Context);

            if (!Enum.TryParse(ToTitleCase(Context.Parameters[0]), out EVolumeShape shape))
                throw new CommandWrongUsageException($"<b>{shape}</b> was not recognized. Available shapes : {EVolumeShape.Cube}, {EVolumeShape.Cylinder}, {EVolumeShape.Sphere}");

            if (!_effectBuilder.Exists(Context.Parameters[0], Context.Parameters[1], Context.Parameters[2]))
                throw new CommandWrongUsageException($"Shape <b>{Context.Parameters[0]}</b> with material <b>{Context.Parameters[1]}</b> and color <b>{Context.Parameters[2]}</b> is not known");

            _volumeEditor.StartEditing(user.Player.Player, shape, Context.Parameters[1], Context.Parameters[2]);

            return UniTask.CompletedTask;
        }

        private string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }
    }
}