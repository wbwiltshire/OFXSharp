using System;
using System.Collections.Generic;
using System.Xml;
namespace OFXSharp
{
    public class InvestmentAccount : Account
    {
        public string BrokerID { get; set; }

        public InvestmentAccount(XmlNode node)
        {
            AccountType = AccountType.INVESTMENT;

            BrokerID = node.GetValue("//BROKERID");

            Transactions = new List<Transaction>();
        }
    }
}
