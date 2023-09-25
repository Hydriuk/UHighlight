using Hydriuk.UnturnedModules.Adapters;
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
        private IHighlightBuilder _highlightBuilder;

        public HighlightSpawner(IServiceAdapter serviceAdapter)
        {
            Task.Run(async () => _highlightBuilder = await serviceAdapter.GetServiceAsync<IHighlightBuilder>());
        }

        public IEnumerable<HighlightedZone> BuildZones(string group) => _highlightBuilder.BuildZones(group);
        public HighlightedZone BuildZone(string group, string name) => _highlightBuilder.BuildZone(group, name);
    }
}