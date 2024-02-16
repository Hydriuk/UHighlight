using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;

namespace UHighlight.OpenMod
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    internal class ConfigurationAdapter : Configuration, IConfigurationAdapter<Configuration>
    {
        public Configuration Configuration { get; private set; } = new Configuration();

        public ConfigurationAdapter(IConfiguration configurator)
        {
            configurator.Bind(Configuration);
        }
    }
}
