using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Core.Plugins;
using OpenMod.Core.Plugins.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHighlight.OpenMod.Adapters
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    internal class ServiceAdapter : IServiceAdapter
    {
        private readonly IServiceProvider _serviceProvider;

        private TaskCompletionSource<Plugin> _plugin;

        public ServiceAdapter(IServiceProvider serviceProvider, Lazy<IPluginAccessor<Plugin>> pluginAccessor)
        {
            _plugin = new TaskCompletionSource<Plugin>();
            _serviceProvider = serviceProvider;

            PluginLoadedListener.PluginLoaded += OnPluginLoaded;
            PluginLoadedListener.PluginUnloaded += OnPluginUnloaded;

            if(pluginAccessor.Value != null && pluginAccessor.Value.Instance != null)
            {
                _plugin.SetResult(pluginAccessor.Value.Instance);
            }
        }

        private void OnPluginLoaded(object? sender, Plugin plugin)
        {
            _plugin.SetResult(plugin);
        }

        private void OnPluginUnloaded(object? sender, Plugin plugin)
        {
            _plugin = new TaskCompletionSource<Plugin>();
        }

        public async Task<TService> GetServiceAsync<TService>()
        {
            Plugin plugin = await _plugin.Task;

            return plugin.ServiceProvider.GetRequiredService<TService>();
        }

        private class PluginLoadedListener : IEventListener<PluginLoadedEvent>, IEventListener<PluginUnloadedEvent>
        {
            public static event EventHandler<Plugin>? PluginLoaded;
            public static event EventHandler<Plugin>? PluginUnloaded;

            public Task HandleEventAsync(object? sender, PluginLoadedEvent @event)
            {
                if(@event.Plugin is Plugin plugin)
                {
                    PluginLoaded?.Invoke(sender, plugin);
                }

                return Task.CompletedTask;
            }

            public Task HandleEventAsync(object? sender, PluginUnloadedEvent @event)
            {
                if (@event.Plugin is Plugin plugin)
                {
                    PluginUnloaded?.Invoke(sender, plugin);
                }

                return Task.CompletedTask;
            }
        }
    }
}
