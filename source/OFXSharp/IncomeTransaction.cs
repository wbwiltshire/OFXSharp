using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace OFXSharp
{
    public class IncomeTransaction : Transaction
    {
        public InvestmentTransaction InvTransaction { get; set; }
        public decimal Total { get; set; }
        public string SecuritySubAccount { get; set; }
        public string FundSubAccount { get; set; }

        public IncomeTransaction()
        {
        }

        public IncomeTransaction(XmlNode node, string currency)
        {
            InvTransaction = new InvestmentTransaction();

            //Look in first child of the context node (.//)
            FITransactionID = node.GetValue(".//FITID");
            InvTransaction.TradeDate = node.GetValue(".//DTTRADE").ToDate();
            InvTransaction.SettleDate = node.GetValue(".//DTSETTLE").ToDate();
            Memo = node.GetValue(".//MEMO");
            InvTransaction.TransactionType = node.GetValue(".//INCOMETYPE");
            Total = Convert.ToDecimal(node.GetValue(".//TOTAL").Trim(), CultureInfo.InvariantCulture);
            SecuritySubAccount = node.GetValue(".//SUBACCTSEC");
            FundSubAccount = node.GetValue(".//SUBACCTFUND");

        }
    }
}
