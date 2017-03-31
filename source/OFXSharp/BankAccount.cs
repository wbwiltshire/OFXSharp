using System;
using System.Collections.Generic;
using System.Xml;

namespace OFXSharp
{
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
    }
}