using Cysharp.Threading.Tasks;
using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;
using UHighlight.API;

[assembly: PluginMetadata("UHighlight", Author = "Hydriuk", Description = "Creates player visible zones", DisplayName = "UHighlight")]

namespace UHighlight.OpenMod
{
    public class UHighlightPlugin : OpenModUnturnedPlugin, IAdaptablePlugin
    {
        public IServiceProvider ServiceProvider { get; private set; }

        private readonly ILogger<UHighlightPlugin> _logger;

        public UHighlightPlugin(IServiceProvider serviceProvider, ILogger<UHighlightPlugin> logger) : base(serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _logger = logger;
        }

        protected override UniTask OnLoadAsync()
        {
            _logger.LogInformation("Loading UHighlight by Hydriuk loaded");

            ServiceProvider.GetRequiredService<IVolumeStore>();

            _logger.LogInformation("UHighlight successfully loaded");

            return UniTask.CompletedTask;
        }

        protected override UniTask OnUnloadAsync()
        {
            _logger.LogInformation("UHighlight Unloaded");

            return UniTask.CompletedTask;
        }
    }
}