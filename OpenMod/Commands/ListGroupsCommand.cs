using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using System.Collections.Generic;
using System.Text;
using UHighlight.API;

namespace UHighlight.OpenMod.Commands
{
    [CommandParent(typeof(HighlightCommand))]
    [Command("listgroups")]
    [CommandAlias("lg")]
    internal class ListGroupsCommand : UnturnedCommand
    {
        private readonly IVolumeStore _volumeStore;

        public ListGroupsCommand(IServiceProvider serviceProvider, IVolumeStore volumeStore) : base(serviceProvider)
        {
            _volumeStore = volumeStore;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            IEnumerable<string> groups = _volumeStore.GetGroups();

            StringBuilder sb = new StringBuilder($"Volumes groups : ");

            foreach (string group in groups)
            {
                sb.Append(group);
                sb.Append(", ");
            }

            user.PrintMessageAsync(sb.ToString());

            return UniTask.CompletedTask;
        }
    }
}