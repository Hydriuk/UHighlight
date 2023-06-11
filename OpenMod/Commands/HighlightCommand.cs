﻿using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHighlight.OpenMod.Commands
{
    [Command("hightlight")]
    [CommandAlias("hl")]
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
