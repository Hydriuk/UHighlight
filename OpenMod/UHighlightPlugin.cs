using Cysharp.Threading.Tasks;
using Hydriuk.OpenModModules;
using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;
using UHighlight.API;

[assembly: PluginMetadata("UHighlight", Author = "Hydriuk", Description = "Creates player visible zones", DisplayName = "UHighlight")]

namespace UHighlight.OpenMod
{
    public class UHighlightPlugin : OpenModUnturnedPlugin
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public UHighlightPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        protected override UniTask OnLoadAsync()
        {
            ServiceProvider.GetRequiredService<IZonePropertyController>();

            return base.OnLoadAsync();
        }
    }

    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
            ServiceRegistrator.ConfigureServices<UHighlightPlugin, Configuration>(openModStartupContext, serviceCollection);
        }
    }
}