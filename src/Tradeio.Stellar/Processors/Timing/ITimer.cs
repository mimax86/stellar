using System;

namespace Tradeio.Stellar.Processors.Timing
{
    public interface ITimer : IDisposable
    {
        void Start();

        void Stop();
    }
}