using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using System.Collections.Generic;
using System.Text;
using UHighlight.API;
using UHighlight.Models;

namespace UHighlight.OpenMod.Commands
{
    [CommandParent(typeof(HighlightCommand))]
    [Command("listvolumes")]
    [CommandAlias("lv")]
    [CommandSyntax("<group>")]
    internal class ListVolumesCommand : UnturnedCommand
    {
        private readonly IVolumeStore _volumeStore;

        public ListVolumesCommand(IServiceProvider serviceProvider, IVolumeStore volumeStore) : base(serviceProvider)
        {
            _volumeStore = volumeStore;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            if (Context.Parameters.Count != 1)
                throw new CommandWrongUsageException(Context);

            IEnumerable<Volume> volumes = _volumeStore.GetVolumes(Context.Parameters[0]);

            StringBuilder sb = new StringBuilder($"{Context.Parameters[0]}'s volumes : ");

            foreach (Volume volume in volumes)
            {
                sb.Append(volume.Name);
                sb.Append(", ");
            }

            user.PrintMessageAsync(sb.ToString());

            return UniTask.CompletedTask;
        }
    }
}