using System;
using System.Runtime.Serialization;

namespace Tradeio.Stellar
{
    public class StellarClientException : Exception
    {
        public StellarClientException()
        {
        }

        public StellarClientException(string message) : base(message)
        {
        }

        public StellarClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StellarClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}