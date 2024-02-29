using Cysharp.Threading.Tasks;
using Hydriuk.OpenModModules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;
using OpenMod.Core.Events;
using OpenMod.Unturned.Plugins;
using SDG.Unturned;
using System;
using System.Threading.Tasks;
using UHighlight.API;

[assembly: PluginMetadata("UHighlight", Author = "Hydriuk", Description = "Creates player visible zones", DisplayName = "UHighlight")]

namespace UHighlight.OpenMod
{
    public class UHighlightPlugin : OpenModUnturnedPlugin
    {
        public IServiceProvider ServiceProvider { get; private set; }

        private readonly ILogger<UHighlightPlugin> _logger;
        private readonly IEventBus _eventBus;

        public UHighlightPlugin(
            IServiceProvider serviceProvider,
            IEventBus eventBus, 
            ILogger<UHighlightPlugin> logger
        ) : base(serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _logger = logger;
            _eventBus = eventBus;
        }

        protected override UniTask OnLoadAsync()
        {
            _eventBus.Subscribe(this, typeof(ServiceRegistrator).Assembly);

            _logger.LogInformation("[LateLoad] - Generating property zones");

            ServiceProvider.GetRequiredService<IZonePropertyController>();

            _logger.LogInformation("[LateLoad] - Property zones loaded");

            return UniTask.CompletedTask;
        }

        protected override UniTask OnUnloadAsync()
        {
            _eventBus.Unsubscribe(this);

            return UniTask.CompletedTask;
        }
    }

    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
            ServiceRegistrator.ConfigureServices<UHighlightPlugin>(openModStartupContext, serviceCollection);
        }
    }
}