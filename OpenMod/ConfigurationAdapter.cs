using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHighlight.OpenMod
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    internal class ConfigurationAdapter : Configuration, IConfigurationAdapter<Configuration>
    {
        public Configuration Configuration { get; private set; }

        public ConfigurationAdapter(IConfiguration configurator)
        {
            Configuration = new Configuration();
            configurator.Bind(Configuration);
        }
    }
}
