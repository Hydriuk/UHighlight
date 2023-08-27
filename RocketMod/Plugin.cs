using Hydriuk.RocketModModules.Adapters;
using Hydriuk.UnturnedModules.Adapters;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHighlight.API;
using UHighlight.DAL;
using UHighlight.RocketMod.Adapters;
using UHighlight.Services;

namespace UHighlight.RocketMod
{
    public class Plugin : RocketPlugin
    {
        public static Plugin Instance { get; private set; }

        private IEnvironmentAdapter _environmentAdapter;
        private IThreadAdapter _threadAdapter;
        private ICoroutineAdapter _coroutineAdapter;

        public IEffectBuilder EffectBuilder { get; private set; }
        public IVolumeStore VolumeStore { get; private set; }
        public IVolumeEditor VolumeEditor { get; private set; }
        public IHighlightBuilder HighlightBuilder { get; private set; }
        public IVolumeTester VolumeTester { get; private set; }
        public IHighlightAdapter HighlightAdapter { get; private set; }

        public Plugin()
        {
            Instance = this;
        }

        protected override void Load()
        {
            _environmentAdapter = new EnvironmentAdapter(this);
            _threadAdapter = new ThreadAdapter();
            _coroutineAdapter = TryAddComponent<CoroutineAdapter>();

            EffectBuilder = new EffectBuilder(_threadAdapter);
            VolumeStore = new VolumeStore(_environmentAdapter);
            VolumeEditor = new VolumeEditor(_coroutineAdapter, EffectBuilder, VolumeStore);
            HighlightBuilder = new HighlightBuilder(VolumeStore);
            VolumeTester = new VolumeTester(HighlightBuilder, EffectBuilder);
            HighlightAdapter = new HighlightAdapter();
        }

        protected override void Unload()
        {
            EffectBuilder.Dispose();
            VolumeEditor.Dispose();

            _coroutineAdapter.Dispose();
        }
    }
}
