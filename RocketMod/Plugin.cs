using Hydriuk.RocketModModules.Adapters;
using Hydriuk.UnturnedModules.Adapters;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHighlight.API;
using UHighlight.Services;

namespace RocketMod
{
    public class Plugin : RocketPlugin
    {
        public static Plugin Instance { get; private set; }

        private IEnvironmentAdapter _environmentAdapter;
        private IThreadAdapter _threadAdapter;
        private ICoroutineAdapter _coroutineAdapter;

        public IEffectBuilder EffectBuilder;
        public IVolumeEditor VolumeEditor { get; private set; }

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
            VolumeEditor = new VolumeEditor(_coroutineAdapter, EffectBuilder);
        }

        protected override void Unload()
        {
            EffectBuilder.Dispose();
            VolumeEditor.Dispose();

            _coroutineAdapter.Dispose();
        }
    }
}
