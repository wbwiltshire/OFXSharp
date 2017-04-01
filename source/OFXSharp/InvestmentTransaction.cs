using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFXSharp
{
    public class InvestmentTransaction
    {
        public InvestmentTransaction()
        {
        }

        public DateTime TradeDate { get; set; }
        public DateTime SettleDate { get; set; }
        public string TransactionType { get; set; }
    }
}
