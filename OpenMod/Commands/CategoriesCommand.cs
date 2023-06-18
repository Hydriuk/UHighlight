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
using UHighlight.Models;

namespace UHighlight.OpenMod.Commands
{
    [CommandParent(typeof(HighlightCommand))]
    [Command("categories")]
    [CommandAlias("cat")]
    public class CategoriesCommand : UnturnedCommand
    {
        private readonly IVolumeStore _volumeStore;

        public CategoriesCommand(IServiceProvider serviceProvider, IVolumeStore volumeStore) : base(serviceProvider)
        {
            _volumeStore = volumeStore;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            IEnumerable<string> categories = _volumeStore.GetCategories();

            StringBuilder sb = new StringBuilder($"Volumes categories : ");

            foreach (string category in categories)
            {
                sb.Append(category);
                sb.Append(", ");
            }

            user.PrintMessageAsync(sb.ToString());

            return UniTask.CompletedTask;
        }
    }
}
