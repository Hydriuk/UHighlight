﻿using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using System.Drawing;
using UHighlight.API;

namespace UHighlight.OpenMod.Commands
{
    [CommandParent(typeof(HighlightCommand))]
    [Command("cancel")]
    [CommandActor(typeof(UnturnedUser))]
    internal class CancelCommand : UnturnedCommand
    {
        private readonly IVolumeEditor _volumeEditor;

        public CancelCommand(IServiceProvider serviceProvider, IVolumeEditor volumeEditor) : base(serviceProvider)
        {
            _volumeEditor = volumeEditor;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            _volumeEditor.StopEditing(user.Player.Player);

            user.PrintMessageAsync("Volume creation cancled", Color.Green);

            return UniTask.CompletedTask;
        }
    }
}