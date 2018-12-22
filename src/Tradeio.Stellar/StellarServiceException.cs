using System;
using System.Runtime.Serialization;

namespace Tradeio.Stellar
{
    public class StellarServiceException : Exception
    {
        public StellarServiceException()
        {
        }

        public StellarServiceException(string message) : base(message)
        {
        }

        public StellarServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StellarServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}