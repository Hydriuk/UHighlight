using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;
using UHighlight.API;

[assembly: PluginMetadata("UHighlight", DisplayName = "UHighlight", Author = "Hydriuk")]

namespace UHighlight.OpenMod
{
    public class Plugin : OpenModUnturnedPlugin
    {
        private readonly IServiceProvider _serviceProvider;

        public Plugin(
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async UniTask OnLoadAsync()
        {

        }

        protected override async UniTask OnUnloadAsync()
        {
        }
    }
}
