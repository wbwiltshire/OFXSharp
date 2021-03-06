﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace OFXSharp
{
    #region Transaction
    public class Transaction
    {
        public string FITransactionID { get; set; }
        public string Memo { get; set; }
    }
    #endregion

    #region BankTransaction
    public class BankTransaction : Transaction
    {
        public OFXTransactionType TransType { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public DateTime TransactionInitializationDate { get; set; }
        public DateTime FundAvaliabilityDate { get; set; }
        public string IncorrectTransactionID { get; set; }
        public TransactionCorrectionType TransactionCorrectionAction { get; set; }
        public string ServerTransactionID { get; set; }
        public string CheckNum { get; set; }
        public string ReferenceNumber { get; set; }
        public string Sic { get; set; }
        public string PayeeID { get; set; }
        public BankAccount TransactionSenderAccount { get; set; }
        public string Currency { get; set; }

        public BankTransaction(XmlNode node, string currency)
        {
            TransType = GetTransactionType(node.GetValue(".//TRNTYPE"));
            Date = node.GetValue(".//DTPOSTED").ToDate();
            TransactionInitializationDate = node.GetValue(".//DTUSER").ToDate();
            FundAvaliabilityDate = node.GetValue(".//DTAVAIL").ToDate();

            try
            {
                Amount = Convert.ToDecimal(node.GetValue(".//TRNAMT"), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new OFXParseException("Transaction Amount unknown", ex);
            }

            try
            {
                FITransactionID = node.GetValue(".//FITID");
            }
            catch (Exception ex)
            {
                throw new OFXParseException("Transaction ID unknown", ex);
            }

            IncorrectTransactionID = node.GetValue(".//CORRECTFITID");

            //If Transaction Correction Action exists, populate
            var tempCorrectionAction = node.GetValue(".//CORRECTACTION");

            TransactionCorrectionAction = !String.IsNullOrEmpty(tempCorrectionAction)
                                             ? GetTransactionCorrectionType(tempCorrectionAction)
                                             : TransactionCorrectionType.NA;

            ServerTransactionID = node.GetValue(".//SRVRTID");
            CheckNum = node.GetValue(".//CHECKNUM");
            ReferenceNumber = node.GetValue(".//REFNUM");
            Sic = node.GetValue(".//SIC");
            PayeeID = node.GetValue(".//PAYEEID");
            Name = node.GetValue(".//NAME");
            Memo = node.GetValue(".//MEMO");

            //If differenct currency to CURDEF, populate currency 
            if (NodeExists(node, ".//CURRENCY"))
                Currency = node.GetValue(".//CURRENCY");
            else if (NodeExists(node, ".//ORIGCURRENCY"))
                Currency = node.GetValue(".//ORIGCURRENCY");
            //If currency not different, set to CURDEF
            else
                Currency = currency;

            ////If senders bank/credit card details avaliable, add
            //if (NodeExists(node, ".//BANKACCTTO"))
            //   TransactionSenderAccount = new BankAccount(node.SelectSingleNode(".//BANKACCTTO"), AccountType.BANK);
            //else if (NodeExists(node, ".//CCACCTTO"))
            //   TransactionSenderAccount = new BankAccount(node.SelectSingleNode(".//CCACCTTO"), AccountType.CC);
        }

        /// <summary>
        /// Returns TransactionType from string version
        /// </summary>
        /// <param name="transactionType">string version of transaction type</param>
        /// <returns>Enum version of given transaction type string</returns>
        private OFXTransactionType GetTransactionType(string transactionType)
        {
            return (OFXTransactionType)Enum.Parse(typeof(OFXTransactionType), transactionType);
        }

        /// <summary>
        /// Returns TransactionCorrectionType from string version
        /// </summary>
        /// <param name="transactionCorrectionType">string version of Transaction Correction Type</param>
        /// <returns>Enum version of given TransactionCorrectionType string</returns>
        private TransactionCorrectionType GetTransactionCorrectionType(string transactionCorrectionType)
        {
            return (TransactionCorrectionType)Enum.Parse(typeof(TransactionCorrectionType), transactionCorrectionType);
        }

        /// <summary>
        /// Checks if a node exists
        /// </summary>
        /// <param name="node">Node to search in</param>
        /// <param name="xpath">XPath to node you want to see if exists</param>
        /// <returns></returns>
        private bool NodeExists(XmlNode node, string xpath)
        {
            return (node.SelectSingleNode(xpath) != null);
        }
    }
    #endregion

    #region CreditCardTransaction
    public class CreditCardTransaction : Transaction
    {
        public OFXTransactionType TransType { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string IncorrectTransactionID { get; set; }
        public TransactionCorrectionType TransactionCorrectionAction { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }

        public CreditCardTransaction(XmlNode node, string currency)
        {
            TransType = GetTransactionType(node.GetValue(".//TRNTYPE"));
            Date = node.GetValue(".//DTPOSTED").ToDate();

            try
            {
                Amount = Convert.ToDecimal(node.GetValue(".//TRNAMT"), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new OFXParseException("Transaction Amount unknown", ex);
            }

            try
            {
                FITransactionID = node.GetValue(".//FITID");
            }
            catch (Exception ex)
            {
                throw new OFXParseException("Transaction ID unknown", ex);
            }

            IncorrectTransactionID = node.GetValue(".//CORRECTFITID");

            //If Transaction Correction Action exists, populate
            var tempCorrectionAction = node.GetValue(".//CORRECTACTION");

            TransactionCorrectionAction = !String.IsNullOrEmpty(tempCorrectionAction)
                                             ? GetTransactionCorrectionType(tempCorrectionAction)
                                             : TransactionCorrectionType.NA;

            Name = node.GetValue(".//NAME");
            if (NodeExists(node, ".//MEMO"))
                Memo = node.GetValue(".//MEMO");

            //If differenct currency to CURDEF, populate currency 
            if (NodeExists(node, ".//CURRENCY"))
                Currency = node.GetValue(".//CURRENCY");
            else if (NodeExists(node, ".//ORIGCURRENCY"))
                Currency = node.GetValue(".//ORIGCURRENCY");
            //If currency not different, set to CURDEF
            else
                Currency = currency;
        }

        /// <summary>
        /// Returns TransactionType from string version
        /// </summary>
        /// <param name="transactionType">string version of transaction type</param>
        /// <returns>Enum version of given transaction type string</returns>
        private OFXTransactionType GetTransactionType(string transactionType)
        {
            return (OFXTransactionType)Enum.Parse(typeof(OFXTransactionType), transactionType);
        }

        /// <summary>
        /// Returns TransactionCorrectionType from string version
        /// </summary>
        /// <param name="transactionCorrectionType">string version of Transaction Correction Type</param>
        /// <returns>Enum version of given TransactionCorrectionType string</returns>
        private TransactionCorrectionType GetTransactionCorrectionType(string transactionCorrectionType)
        {
            return (TransactionCorrectionType)Enum.Parse(typeof(TransactionCorrectionType), transactionCorrectionType);
        }

        /// <summary>
        /// Checks if a node exists
        /// </summary>
        /// <param name="node">Node to search in</param>
        /// <param name="xpath">XPath to node you want to see if exists</param>
        /// <returns></returns>
        private bool NodeExists(XmlNode node, string xpath)
        {
            return (node.SelectSingleNode(xpath) != null);
        }
    }
    #endregion Credit Card Transaction

    #region IncomeTransaction
    public class IncomeTransaction : Transaction
    {
        public InvestmentTransaction InvTransaction { get; set; }

        public IncomeTransaction(XmlNode node, string currency)
        {
            InvTransaction = new InvestmentTransaction();

            //Look in first child of the context node (.//)
            FITransactionID = node.GetValue(".//FITID");
            Memo = node.GetValue(".//MEMO");
            InvTransaction.TradeDate = node.GetValue(".//DTTRADE").ToDate();
            InvTransaction.SettleDate = node.GetValue(".//DTSETTLE").ToDate();
            InvTransaction.UniqueID = node.GetValue(".//SECID//UNIQUEID");
            InvTransaction.UniqueIDType = node.GetValue(".//SECID//UNIQUEIDTYPE");
            InvTransaction.TransactionType = node.GetValue(".//INCOMETYPE");
            InvTransaction.Total = Convert.ToDecimal(node.GetValue(".//TOTAL").Trim(), CultureInfo.InvariantCulture);
            InvTransaction.SecuritySubAccount = node.GetValue(".//SUBACCTSEC");
            InvTransaction.FundSubAccount = node.GetValue(".//SUBACCTFUND");

        }
    }
    #endregion

    #region BuySellStockTransaction
    public class BuySellStockTransaction : Transaction
    {
        public InvestmentTransaction InvTransaction { get; set; }
        public decimal Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Markup { get; set; }
        public decimal Markdown { get; set; }
        public decimal Commission { get; set; }
        public decimal Fees { get; set; }

        public BuySellStockTransaction(XmlNode node, string currency)
        {
            InvTransaction = new InvestmentTransaction();

            InvTransaction.TransactionType = node.Name;
            //Look in first child of the context node (.//)
            FITransactionID = node.GetValue(".//FITID");
            Memo = node.GetValue(".//MEMO");
            Units = Convert.ToDecimal(node.GetValue(".//UNITS").Trim(), CultureInfo.InvariantCulture);
            UnitPrice = Convert.ToDecimal(node.GetValue(".//UNITPRICE").Trim(), CultureInfo.InvariantCulture);
            switch (InvTransaction.TransactionType)
            {
                case "BUYSTOCK":
                    Markup = Convert.ToDecimal(node.GetValue(".//MARKUP").Trim(), CultureInfo.InvariantCulture);
                    break;
                case "SELLSTOCK":
                    Markdown = Convert.ToDecimal(node.GetValue(".//MARKDOWN").Trim(), CultureInfo.InvariantCulture);
                    break;
            }
            Commission = Convert.ToDecimal(node.GetValue(".//COMMISSION").Trim(), CultureInfo.InvariantCulture);
            Fees = Convert.ToDecimal(node.GetValue(".//FEES").Trim(), CultureInfo.InvariantCulture);
            InvTransaction.TradeDate = node.GetValue(".//DTTRADE").ToDate();
            InvTransaction.SettleDate = node.GetValue(".//DTSETTLE").ToDate();
            InvTransaction.UniqueID = node.GetValue(".//SECID//UNIQUEID");
            InvTransaction.UniqueIDType = node.GetValue(".//SECID//UNIQUEIDTYPE");
            InvTransaction.Total = Convert.ToDecimal(node.GetValue(".//TOTAL").Trim(), CultureInfo.InvariantCulture);
            InvTransaction.SecuritySubAccount = node.GetValue(".//SUBACCTSEC");
            InvTransaction.FundSubAccount = node.GetValue(".//SUBACCTFUND");

        }
    }
    #endregion

    #region BuySellStockOptionTransaction
    public class BuySellStockOptionTransaction : Transaction
    {
        public InvestmentTransaction InvTransaction { get; set; }
        public decimal Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Markup { get; set; }
        public decimal Markdown { get; set; }
        public decimal Commission { get; set; }
        public decimal Fees { get; set; }
        public string Type { get; set; }

        public BuySellStockOptionTransaction(XmlNode node, string currency)
        {
            InvTransaction = new InvestmentTransaction();

            InvTransaction.TransactionType = node.Name;
            //Look in first child of the context node (.//)
            FITransactionID = node.GetValue(".//FITID");
            Memo = node.GetValue(".//MEMO");
            Units = Convert.ToDecimal(node.GetValue(".//UNITS").Trim(), CultureInfo.InvariantCulture);
            UnitPrice = Convert.ToDecimal(node.GetValue(".//UNITPRICE").Trim(), CultureInfo.InvariantCulture);
            switch (InvTransaction.TransactionType)
            {
                case "BUYOPT":
                    Markup = Convert.ToDecimal(node.GetValue(".//MARKUP").Trim(), CultureInfo.InvariantCulture);
                    Type = node.GetValue(".//OPTBUYTYPE");
                    break;
                case "SELLOPT":
                    Markdown = Convert.ToDecimal(node.GetValue(".//MARKDOWN").Trim(), CultureInfo.InvariantCulture);
                    Type = node.GetValue(".//OPTSELLTYPE");
                    break;
            }
            Commission = Convert.ToDecimal(node.GetValue(".//COMMISSION").Trim(), CultureInfo.InvariantCulture);
            Fees = Convert.ToDecimal(node.GetValue(".//FEES").Trim(), CultureInfo.InvariantCulture);
            Type = node.GetValue(".//OPTBUYTYPE");
            InvTransaction.TradeDate = node.GetValue(".//DTTRADE").ToDate();
            InvTransaction.SettleDate = node.GetValue(".//DTSETTLE").ToDate();
            InvTransaction.UniqueID = node.GetValue(".//SECID//UNIQUEID");
            InvTransaction.UniqueIDType = node.GetValue(".//SECID//UNIQUEIDTYPE");
            InvTransaction.Total = Convert.ToDecimal(node.GetValue(".//TOTAL").Trim(), CultureInfo.InvariantCulture);
            InvTransaction.SecuritySubAccount = node.GetValue(".//SUBACCTSEC");
            InvTransaction.FundSubAccount = node.GetValue(".//SUBACCTFUND");

        }
    }
    #endregion

    #region StockTransferTransaction
    public class StockTransferTransaction : Transaction
    {
        public InvestmentTransaction InvTransaction { get; set; }
        public decimal Units { get; set; }
        public decimal UnitPrice { get; set; }
        public string Action { get; set; }

        public StockTransferTransaction(XmlNode node, string currency)
        {
            InvTransaction = new InvestmentTransaction();

            InvTransaction.TransactionType = node.Name;
            //Look in first child of the context node (.//)
            FITransactionID = node.GetValue(".//FITID");
            Memo = node.GetValue(".//MEMO");
            Units = Convert.ToDecimal(node.GetValue(".//UNITS").Trim(), CultureInfo.InvariantCulture);
            UnitPrice = Convert.ToDecimal(node.GetValue(".//UNITPRICE").Trim(), CultureInfo.InvariantCulture);
            Action = node.GetValue(".//TFERACTION");
            InvTransaction.TradeDate = node.GetValue(".//DTTRADE").ToDate();
            InvTransaction.SettleDate = node.GetValue(".//DTSETTLE").ToDate();
            InvTransaction.UniqueID = node.GetValue(".//SECID//UNIQUEID");
            InvTransaction.UniqueIDType = node.GetValue(".//SECID//UNIQUEIDTYPE");
            InvTransaction.Total = 0;
            InvTransaction.SecuritySubAccount = node.GetValue(".//SUBACCTSEC");
            
        }
    }
    #endregion

    #region InvestmentTransaction
    public class InvestmentTransaction
    {
        public string UniqueID { get; set; }
        public string UniqueIDType { get; set; }
        public decimal Total { get; set; }
        public DateTime TradeDate { get; set; }
        public DateTime SettleDate { get; set; }
        public string TransactionType { get; set; }
        public string SecuritySubAccount { get; set; }
        public string FundSubAccount { get; set; }

    }
    #endregion

    #region InvestmentTransferTransaction (Cash)
    public class InvestmentTransferTransaction : Transaction
    {
        public string TransactionType { get; set; }
        public DateTime DatePosted { get; set; }
        public decimal Total { get; set; }
        public string Name { get; set; }
        public string FundSubAccount { get; set; }

        public InvestmentTransferTransaction(XmlNode node, string currency)
        {
            //Look in first child of the context node (.//)
            FITransactionID = node.GetValue(".//FITID");
            Memo = node.GetValue(".//MEMO");
            TransactionType = node.GetValue(".//TRNTYPE");
            DatePosted = node.GetValue(".//DTPOSTED").ToDate();
            Total = Convert.ToDecimal(node.GetValue(".//TRNAMT").Trim(), CultureInfo.InvariantCulture);
            Name = node.GetValue(".//NAME");
            FundSubAccount = node.GetValue(".//SUBACCTFUND");
        }
    }
    #endregion
}
