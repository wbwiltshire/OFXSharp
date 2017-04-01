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

            AccountID = node.GetValue("//ACCTID");
            BrokerID = node.GetValue("//BROKERID");

            Transactions = new List<Transaction>();
        }

        /// <summary>
        /// Returns list of all transactions in OFX document
        /// </summary>
        /// <param name="doc">OFX document</param>
        /// <returns>List of transactions found in OFX document</returns>
        public override void ProcessTransactions(OFXDocumentParser parser, OFXDocument ofx, XmlDocument doc)
        {
            XmlNodeList transactionNodes = null;
            var xpath = parser.GetXPath(ofx.AccType, OFXDocumentParser.OFXSection.TRANSACTIONS);

            ofx.StatementStart = doc.GetValue(xpath + "//DTSTART").ToDate();
            ofx.StatementEnd = doc.GetValue(xpath + "//DTEND").ToDate();

            //Process Income Transactions
            transactionNodes = doc.SelectNodes(xpath + "//INCOME");
            foreach (XmlNode node in transactionNodes)
                Transactions.Add(new IncomeTransaction(node, ofx.Currency));

            //Process BuyType Transactions
            transactionNodes = doc.SelectNodes(xpath + "//BUYSTOCK");
            foreach (XmlNode node in transactionNodes)
                Transactions.Add(new BuyStockTransaction(node, ofx.Currency));
        }
    }
}
