#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using Hydriuk.UnturnedModules.Adapters;
using SDG.Unturned;
using System.Collections.Generic;
using System.Threading.Tasks;
using UHighlight.API;
using UHighlight.Components;
using UHighlight.Models;

namespace UHighlight.Services
{
#if OPENMOD
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    internal class HighlightSpawner : IHighlightSpawner
    {
        private readonly Task<IHighlightBuilder> _builderTask;
        private readonly TaskCompletionSource<bool> _levelLoadedTask = new TaskCompletionSource<bool>();

        public HighlightSpawner(IServiceAdapter serviceAdapter)
        {
            _builderTask = serviceAdapter.GetServiceAsync<IHighlightBuilder>();

            if (Level.isLoaded)
                SetLevelLoaded();
            else
                Level.onPostLevelLoaded += OnLevelLoaded;
        }

        public void Dispose()
        {
            Level.onPostLevelLoaded -= OnLevelLoaded;
        }

        private void OnLevelLoaded(int level) => SetLevelLoaded();
        private void SetLevelLoaded() => _levelLoadedTask.SetResult(true);

        public async Task<IEnumerable<HighlightedZone>> BuildZones(string group, float customSize = -1)
        {
            await _levelLoadedTask.Task;
            IHighlightBuilder highlightBuilder = await _builderTask;

            return await highlightBuilder.BuildZones(group, customSize);
        }

        public async Task<HighlightedZone> BuildZone(string group, string name, float customSize = -1)
        {
            await _levelLoadedTask.Task;
            IHighlightBuilder highlightBuilder = await _builderTask;

            return await highlightBuilder.BuildZone(group, name, customSize);
        }

        public async Task<HighlightedZone> BuildZone(Volume volume)
        {
            await _levelLoadedTask.Task;
            IHighlightBuilder highlightBuilder = await _builderTask;

            return await highlightBuilder.BuildZone(volume);
        }
    }
}