using Cysharp.Threading.Tasks;
using Hydriuk.UnturnedModules.Adapters;
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
    public class UHighlightPlugin : OpenModUnturnedPlugin, IAdaptablePlugin
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public UHighlightPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        protected override async UniTask OnLoadAsync()
        {

        }

        protected override async UniTask OnUnloadAsync()
        {
        }
    }
}
