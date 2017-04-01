using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace OFXSharp
{
    public class BuyStockTransaction : Transaction
    {
        public InvestmentTransaction InvTransaction { get; set; }
        public decimal Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Markup { get; set; }
        public decimal Commission { get; set; }
        public decimal Fees { get; set; }
        public decimal Total { get; set; }
        public string SecuritySubAccount { get; set; }
        public string FundSubAccount { get; set; }

        public BuyStockTransaction()
        {
        }

        public BuyStockTransaction(XmlNode node, string currency)
        {
            InvTransaction = new InvestmentTransaction();

            //Look in first child of the context node (.//)
            InvTransaction.TransactionType = "STOCKBUY";
            FITransactionID = node.GetValue(".//FITID");
            InvTransaction.TradeDate = node.GetValue(".//DTTRADE").ToDate();
            InvTransaction.SettleDate = node.GetValue(".//DTSETTLE").ToDate();
            Memo = node.GetValue(".//MEMO");
            Units = Convert.ToDecimal(node.GetValue(".//UNITS").Trim(), CultureInfo.InvariantCulture);
            UnitPrice = Convert.ToDecimal(node.GetValue(".//UNITPRICE").Trim(), CultureInfo.InvariantCulture);
            Markup = Convert.ToDecimal(node.GetValue(".//MARKUP").Trim(), CultureInfo.InvariantCulture);
            Commission = Convert.ToDecimal(node.GetValue(".//COMMISSION").Trim(), CultureInfo.InvariantCulture);
            Fees = Convert.ToDecimal(node.GetValue(".//FEES").Trim(), CultureInfo.InvariantCulture);
            Total = Convert.ToDecimal(node.GetValue(".//TOTAL").Trim(), CultureInfo.InvariantCulture);
            SecuritySubAccount = node.GetValue(".//SUBACCTSEC");
            FundSubAccount = node.GetValue(".//SUBACCTFUND");

        }
    }
}
