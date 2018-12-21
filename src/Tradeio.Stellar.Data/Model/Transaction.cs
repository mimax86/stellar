using System;
using System.Collections.Generic;
using System.Text;

namespace Tradeio.Stellar.Data.Model
{
    public class Transaction
    {
        public long TraderId { get; set; }

        public string Hash { get; set; }

        public decimal Amount { get; set; }
    }
}