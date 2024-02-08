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

        public UHighlightPlugin(
            IServiceProvider serviceProvider, 
            ILogger<UHighlightPlugin> logger
        ) : base(serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _logger = logger;
        }
    }

    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
            ServiceRegistrator.ConfigureServices<UHighlightPlugin>(openModStartupContext, serviceCollection);
        }
    }

    [EventListenerLifetime(ServiceLifetime.Singleton)]
    public class OpenModLoadedEvent : IEventListener<OpenModInitializedEvent>, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OpenModLoadedEvent> _logger;

        public OpenModLoadedEvent(IServiceProvider serviceProvider, ILogger<OpenModLoadedEvent> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task HandleEventAsync(object? sender, OpenModInitializedEvent @event)
        {
            if (Level.isLoaded)
                LateLoad();
            else
                Level.onPostLevelLoaded += OnLevelLoaded;

            return Task.CompletedTask;
        }

        private void OnLevelLoaded(int level) => LateLoad();
        private void LateLoad()
        {
            _logger.LogInformation("[LateLoad] - Generating property zones");

            _serviceProvider.GetRequiredService<IZonePropertyController>();

            _logger.LogInformation("[LateLoad] - Property zones loaded");
        }

        public void Dispose()
        {
            Level.onPostLevelLoaded -= OnLevelLoaded;
        }
    }
}