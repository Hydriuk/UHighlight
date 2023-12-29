using System;
using HarmonyLib;
using Hydriuk.RocketModModules;
using Hydriuk.RocketModModules.Adapters;
using Hydriuk.UnturnedModules.Adapters;
using Rocket.Core;
using Rocket.Core.Plugins;
using UHighlight.API;
using UHighlight.DAL;
using UHighlight.RocketMod.Adapters;
using UHighlight.Services;

namespace UHighlight.RocketMod
{
    public class UHighlightPlugin : RocketPlugin<ConfigurationAdapter>
    {
        public static UHighlightPlugin Instance { get; private set; }

        private Harmony _harmony;

        private ServiceRegistrator _serviceRegistrator;

        [PluginService] private EnvironmentAdapter _environmentAdapter;
        [PluginService] private ConfigurationAdapter<Configuration> _configurationAdapter;
        [PluginService] private ThreadAdapter _threadAdapter;
        [PluginService] private ServiceAdapter _serviceAdapter;

        [PluginService] internal ChatAdapter ChatAdapter { get; private set; }
        [PluginService] internal EffectBuilder EffectBuilder { get; private set; }
        [PluginService] internal VolumeStore VolumeStore { get; private set; }
        [PluginService] internal VolumeEditor VolumeEditor { get; private set; }
        [PluginService] internal HighlightBuilder HighlightBuilder { get; private set; }
        [PluginService] internal VolumeTester VolumeTester { get; private set; }
        [PluginService] internal AdminUIManager AdminUIManager { get; private set; }

        // Public APIs instances are not public to prevent access before they are instanciated. They should be retreived using IServiceAdapter
        [PluginService] internal HighlightCommands HighlightCommands { get; private set; }
        [PluginService] internal HighlightSpawner HighlightSpawner { get; private set; }

        public UHighlightPlugin()
        {
            Instance = this;
        }

        protected override void Load()
        {
            _serviceRegistrator = new ServiceRegistrator(this);

            _harmony = new Harmony("Hydriuk.UHighlight");
            _harmony.PatchAll();
        }

        protected override void Unload()
        {
            _serviceRegistrator.Dispose();

            _harmony?.UnpatchAll("Hydriuk.UHighlight");
        }
    }
}