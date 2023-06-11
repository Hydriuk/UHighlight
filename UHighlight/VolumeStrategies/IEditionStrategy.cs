using Hydriuk.UnturnedModules.PlayerKeys;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using UHighlight.Models;

namespace UHighlight.VolumeStrategies
{
    public interface IEditionStrategy : IDisposable
    {
        Volume? Build();
        void Cancel();
    }
}
