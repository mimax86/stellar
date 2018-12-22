using System;

namespace Tradeio.Stellar.Processors.Timing
{
    public interface ITimerFactory
    {
        ITimer Create(Action handler);
    }
}