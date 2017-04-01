using System;
using System.Collections.Generic;
using System.Xml;

namespace OFXSharp
{
    public abstract class Account
    {
        public string AccountID { get; set; }
        public AccountType AccountType { get; set; }
        public IList<Transaction> Transactions { get; set; }

        public Account()
        { }

        public abstract void ProcessTransactions(OFXDocumentParser parser, OFXDocument ofx, XmlDocument doc);
    }
}
