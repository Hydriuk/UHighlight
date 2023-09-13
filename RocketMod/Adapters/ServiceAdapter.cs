using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UHighlight.RocketMod.Adapters
{
    internal class ServiceAdapter : IServiceAdapter
    {
        private readonly Plugin _plugin;

        public bool IsPluginLoaded { get; private set; }

        public event EventHandler PluginLoaded;

        public ServiceAdapter(Plugin plugin)
        {
            _plugin = plugin;
        }

        public Task<TService> GetServiceAsync<TService>()
        {
            PropertyInfo serviceInfo = typeof(Plugin)
                .GetProperties()
                .Where(property => property.PropertyType == typeof(TService))
                .FirstOrDefault();

            return Task.FromResult((TService)serviceInfo.GetValue(_plugin));
        }
    }
}
