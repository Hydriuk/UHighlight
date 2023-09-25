﻿using Hydriuk.RocketModModules.Adapters;
using Hydriuk.UnturnedModules.Adapters;
using Rocket.Core.Plugins;
using UHighlight.API;
using UHighlight.DAL;
using UHighlight.RocketMod.Adapters;
using UHighlight.Services;

namespace UHighlight.RocketMod
{
    public class UHighlightPlugin : RocketPlugin, IAdaptablePlugin
    {
        public static UHighlightPlugin Instance { get; private set; }

        private IEnvironmentAdapter _environmentAdapter;
        private IThreadAdapter _threadAdapter;
        private ICoroutineAdapter _coroutineAdapter;
        private IServiceAdapter _serviceAdapter;

        internal IEffectBuilder EffectBuilder { get; private set; }
        internal IVolumeStore VolumeStore { get; private set; }
        internal IVolumeEditor VolumeEditor { get; private set; }
        internal IHighlightBuilder HighlightBuilder { get; private set; }
        internal IVolumeTester VolumeTester { get; private set; }

        // Public APIs instances are not public to prevent access before they are instanciated. They should be retreived using IServiceAdapter
        internal IHighlightCommands HighlightCommands { get; private set; }
        internal IHighlightSpawner HighlightSpawner { get; private set; }

        public UHighlightPlugin()
        {
            Instance = this;
        }

        protected override void Load()
        {
            _environmentAdapter = new EnvironmentAdapter(this);
            _threadAdapter = new ThreadAdapter();
            _coroutineAdapter = TryAddComponent<CoroutineAdapter>();
            _serviceAdapter = new ServiceAdapter(this);

            EffectBuilder = new EffectBuilder(_threadAdapter);
            VolumeStore = new VolumeStore(_environmentAdapter);
            VolumeEditor = new VolumeEditor(_coroutineAdapter, EffectBuilder, VolumeStore);
            HighlightBuilder = new HighlightBuilder(VolumeStore);
            VolumeTester = new VolumeTester(HighlightBuilder, EffectBuilder);
            HighlightCommands = new HighlightCommands();
            HighlightSpawner = new HighlightSpawner(_serviceAdapter);
        }

        protected override void Unload()
        {
            EffectBuilder.Dispose();
            VolumeEditor.Dispose();
            VolumeStore.Dispose();
            VolumeTester.Dispose();

            _coroutineAdapter.Dispose();
            _serviceAdapter.Dispose();
        }
    }
}