using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHighlight.OpenMod.Adapters
{
    [ServiceImplementation]
    public class ServiceAdapter : IServiceAdapter
    {
        private readonly Lazy<IPluginAccessor<Plugin>> _pluginAccessor;

        public ServiceAdapter(Lazy<IPluginAccessor<Plugin>> pluginAccessor)
        {
            _pluginAccessor = pluginAccessor;
        }

        public TService GetService<TService>()
        {
            if (_pluginAccessor.Value == null || _pluginAccessor.Value.Instance == null)
                throw new Exception("Plugin not instanciated");

            return _pluginAccessor.Value.Instance.ServiceProvider.GetRequiredService<TService>();
        }
    }
}
