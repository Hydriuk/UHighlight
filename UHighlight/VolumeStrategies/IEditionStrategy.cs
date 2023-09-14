using System;
using UHighlight.Models;

namespace UHighlight.VolumeStrategies
{
    public interface IEditionStrategy : IDisposable
    {
        Volume? Build();
        void Cancel();
    }
}