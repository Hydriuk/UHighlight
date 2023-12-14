using Hydriuk.UnturnedModules.Adapters;
using System;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Collections.Generic;
using System.Threading.Tasks;
using UHighlight.API;
using UHighlight.Components;
#if OPENMOD
using UHighlight.OpenMod;
#elif ROCKETMOD
using UHighlight.RocketMod;
#endif

namespace UHighlight.Services
{
#if OPENMOD
    [ServiceImplementation]
#endif
    internal class HighlightSpawner : IHighlightSpawner
    {
        private TaskCompletionSource<bool> _serviceLoaded;

        private IHighlightBuilder? _highlightBuilder;

        public HighlightSpawner(IServiceAdapter serviceAdapter)
        {
            _serviceLoaded = new TaskCompletionSource<bool>();

            Task.Run(async () => {
                _highlightBuilder = await serviceAdapter.GetServiceAsync<UHighlightPlugin, IHighlightBuilder>();
                _serviceLoaded.SetResult(true);
            });
        }

        public async Task<IEnumerable<HighlightedZone>> BuildZones(string group, float customSize = -1)
        {
            if(!_serviceLoaded.Task.IsCompleted) 
            { 
                await _serviceLoaded.Task;
            }

            if (_highlightBuilder == null)
                throw new Exception("[UHighlight] - The Highlight builder service could not be loaded");

            return _highlightBuilder.BuildZones(group, customSize);
        }

        public async Task<HighlightedZone> BuildZone(string group, string name, float customSize = -1)
        {
            if (!_serviceLoaded.Task.IsCompleted)
            {
                await _serviceLoaded.Task;
            }

            if (_highlightBuilder == null)
                throw new Exception("[UHighlight] - The Highlight builder service could not be loaded");

            return _highlightBuilder.BuildZone(group, name, customSize);
        }
    }
}