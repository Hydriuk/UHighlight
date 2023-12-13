using System;
using UHighlight.Models;

namespace UHighlight.EditionStrategies
{
    public interface IEditionStrategy : IDisposable
    {
        Volume? Build();
        void Cancel();
        void SetSize(float size);
    }
}