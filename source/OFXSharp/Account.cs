using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFXSharp
{
    public class Account
    {
        public Account()
        { }
        public string AccountID { get; set; }
        public AccountType AccountType { get; set; }

        public IList<Transaction> Transactions { get; set; }
    }
}
