using System;
using System.Collections.Generic;
using System.Xml;

namespace OFXSharp
{
    #region Account
    public abstract class Account
    {
        public string AccountID { get; set; }
        public AccountType AccountType { get; set; }
        public IList<Transaction> Transactions { get; set; }

        public Account()
        { }

        public abstract void ImportTransactions(OFXDocumentParser parser, OFXDocument ofx, XmlDocument doc);
    }
    #endregion

    #region BankAccount
    public class BankAccount : Account
    {
        #region Bank Only
        public string AccountKey { get; set; }

        public string BankID { get; set; }

        public string BranchID { get; set; }

        public BankAccountType BankAccountType { get; set; }
        #endregion

        public BankAccount(XmlNode node)
        {
            AccountType = AccountType.BANK;

            AccountID = node.GetValue("//ACCTID");
            AccountKey = node.GetValue("//ACCTKEY");
            BankID = node.GetValue("//BANKID");
            BranchID = node.GetValue("//BRANCHID");

            //Get Bank Account Type from XML
            string bankAccountType = node.GetValue("//ACCTTYPE");

            //Check that it has been set
            if (String.IsNullOrEmpty(bankAccountType))
                throw new OFXParseException("Bank Account type unknown");

            //Set bank account enum
            BankAccountType = bankAccountType.GetBankAccountType();

            Transactions = new List<Transaction>();
        }

        /// <summary>
        /// Returns list of all transactions in OFX document
        /// </summary>
        /// <param name="doc">OFX document</param>
        /// <returns>List of transactions found in OFX document</returns>
        public override void ImportTransactions(OFXDocumentParser parser, OFXDocument ofx, XmlDocument doc)
        {
            XmlNodeList transactionNodes = null;
            var xpath = parser.GetXPath(AccountType, OFXDocumentParser.OFXSection.TRANSACTIONS);

            ofx.StatementStart = doc.GetValue(xpath + "//DTSTART").ToDate();
            ofx.StatementEnd = doc.GetValue(xpath + "//DTEND").ToDate();

            transactionNodes = doc.SelectNodes(xpath + "//STMTTRN");

            foreach (XmlNode node in transactionNodes)
                Transactions.Add(new BankTransaction(node, ofx.Currency));
        }
    }
    #endregion

    #region CreditCardAccount
    public class CreditCardAccount : Account
    {
        #region CreditCard Only
        #endregion

        public CreditCardAccount(XmlNode node)
        {
            AccountType = AccountType.CC;

            AccountID = node.GetValue("//ACCTID");

            //Get Bank Account Type from XML
            //string ccAccountType = node.GetValue("//ACCTTYPE");
            string ccAccountType = "CREDITCARD";

            //Check that it has been set
            if (String.IsNullOrEmpty(ccAccountType))
                throw new OFXParseException("Credit Card Account type unknown");

            Transactions = new List<Transaction>();
        }

        /// <summary>
        /// Returns list of all transactions in OFX document
        /// </summary>
        /// <param name="doc">OFX document</param>
        /// <returns>List of transactions found in OFX document</returns>
        public override void ImportTransactions(OFXDocumentParser parser, OFXDocument ofx, XmlDocument doc)
        {
            XmlNodeList transactionNodes = null;
            var xpath = parser.GetXPath(AccountType, OFXDocumentParser.OFXSection.TRANSACTIONS);

            ofx.StatementStart = doc.GetValue(xpath + "//DTSTART").ToDate();
            ofx.StatementEnd = doc.GetValue(xpath + "//DTEND").ToDate();

            transactionNodes = doc.SelectNodes(xpath + "//STMTTRN");

            foreach (XmlNode node in transactionNodes)
                Transactions.Add(new CreditCardTransaction(node, ofx.Currency));
        }
    }
    #endregion

    #region Investment Account
    public class InvestmentAccount : Account
    {
        public string BrokerID { get; set; }

        public IList<SECInfo> StockQuotes { get; set; }
        public IList<Position> Positions { get; set; }

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
        public override void ImportTransactions(OFXDocumentParser parser, OFXDocument ofx, XmlDocument doc)
        {
            XmlNodeList transactionNodes = null;
            var xpath = parser.GetXPath(ofx.AccType, OFXDocumentParser.OFXSection.TRANSACTIONS);

            ofx.StatementStart = doc.GetValue(xpath + "//DTSTART").ToDate();
            ofx.StatementEnd = doc.GetValue(xpath + "//DTEND").ToDate();

            //Import Income Transactions
            transactionNodes = doc.SelectNodes(xpath + "//INCOME");
            foreach (XmlNode node in transactionNodes)
                Transactions.Add(new IncomeTransaction(node, ofx.Currency));

            //Import Cash Transfer Transactions
            transactionNodes = doc.SelectNodes(xpath + "//INVBANKTRAN");
            foreach (XmlNode node in transactionNodes)
                Transactions.Add(new InvestmentTransferTransaction(node, ofx.Currency));

            //Import Stock Transfer Transactions
            transactionNodes = doc.SelectNodes(xpath + "//TRANSFER");
            foreach (XmlNode node in transactionNodes)
                Transactions.Add(new StockTransferTransaction(node, ofx.Currency));

            //Import Buy Stock Transactions
            transactionNodes = doc.SelectNodes(xpath + "//BUYSTOCK");
            foreach (XmlNode node in transactionNodes)
                Transactions.Add(new BuySellStockTransaction(node, ofx.Currency));

            //Import Buy Option Transactions
            transactionNodes = doc.SelectNodes(xpath + "//BUYOPT");
            foreach (XmlNode node in transactionNodes)
                Transactions.Add(new BuySellStockOptionTransaction(node, ofx.Currency));

            //Import Sell Stock Transactions
            transactionNodes = doc.SelectNodes(xpath + "//SELLSTOCK");
            foreach (XmlNode node in transactionNodes)
                Transactions.Add(new BuySellStockTransaction(node, ofx.Currency));

            //Import Buy Option Transactions
            transactionNodes = doc.SelectNodes(xpath + "//SELLOPT");
            foreach (XmlNode node in transactionNodes)
                Transactions.Add(new BuySellStockOptionTransaction(node, ofx.Currency));

            //Import Sell Other Stock Transactions
            transactionNodes = doc.SelectNodes(xpath + "//SELLOTHER");
            foreach (XmlNode node in transactionNodes)
                Transactions.Add(new BuySellStockTransaction(node, ofx.Currency));
        }

        public void ImportPositions(string xpath, OFXDocument ofx, XmlDocument doc)
        {
            XmlNodeList positionNodes = null;
            Positions = new List<Position>();

            //Import Position Transactions
            positionNodes = doc.SelectNodes(xpath + "//POSSTOCK");
            foreach (XmlNode node in positionNodes)
                Positions.Add(new Position(node));
            positionNodes = doc.SelectNodes(xpath + "//POSOTHER");
            foreach (XmlNode node in positionNodes)
                Positions.Add(new Position(node));
            positionNodes = doc.SelectNodes(xpath + "//POSMF");
            foreach (XmlNode node in positionNodes)
                Positions.Add(new Position(node));
        }

        public void ImportSECList(XmlNode doc)
        {
            XmlNodeList quoteNodes = null;
            StockQuotes = new List<SECInfo>();

            //Import Stock Quotes Transactions
            quoteNodes = doc.SelectNodes("//STOCKINFO");
            foreach (XmlNode node in quoteNodes)
                StockQuotes.Add(new StockInfo(node));

            //Import Mutual Fund Info
            quoteNodes = doc.SelectNodes("//MFINFO");
            foreach (XmlNode node in quoteNodes)
                StockQuotes.Add(new MutualFundInfo(node));

            //Import Money Market Info
            quoteNodes = doc.SelectNodes("//OTHERINFO");
            foreach (XmlNode node in quoteNodes)
                StockQuotes.Add(new OtherInfo(node));
        }
    }
    #endregion

    #region APAccount
    public class APAccount : Account
    {
        public APAccount(XmlNode node)
        {
            throw new OFXParseException("AP Account type not supported");
        }
        public override void ImportTransactions(OFXDocumentParser parser, OFXDocument ofx, XmlDocument doc)
        {
        }
    }
    #endregion

    #region ARAccount
    public class ARAccount : Account
    {
        public ARAccount(XmlNode node)
        {
            throw new OFXParseException("AR Account type not supported");
        }

        public override void ImportTransactions(OFXDocumentParser parser, OFXDocument ofx, XmlDocument doc)
        {
        }
    }
    #endregion
}
