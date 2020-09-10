using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace OFXSharp.XUnit
{
    public class RegressionTests
    {
        [Fact]
        public void OFXBankTest()
        {
            string testFile = "santander.ofx";

            var parser = new OFXDocumentParser();
            var ofxDocument = parser.Import(new FileStream(testFile, FileMode.Open));
            Assert.NotNull(ofxDocument);
            var balance = (BankBalance)ofxDocument.Balance;
            Assert.True(balance.LedgerBalance > 0);
            Assert.NotNull(ofxDocument.Account.Transactions);
            Assert.True(ofxDocument.Account.Transactions.Count > 0);
            Console.WriteLine("Test finished");
        }

        [Fact]
        public void OFXCreditCardTest()
        {
            string testFile = "creditcard.ofx";

            var parser = new OFXDocumentParser();
            var ofxDocument = parser.Import(new FileStream(testFile, FileMode.Open));
            Assert.NotNull(ofxDocument);
            var balance = (CreditCardBalance)ofxDocument.Balance;
            Assert.NotNull(ofxDocument.Account.Transactions);
            Assert.True(ofxDocument.Account.Transactions.Count > 0);
            Console.WriteLine("Test finished");
        }

        [Fact]
        public void OFXInvestmentTest()
        {
            //string testFile = "investment.ofx";
            string testFile = "import.ofx";

            var parser = new OFXDocumentParser();
            var ofxDocument = parser.Import(new FileStream(testFile, FileMode.Open));
            Assert.NotNull(ofxDocument);
            var balance = (InvestmentBalance)ofxDocument.Balance;
            Assert.True(balance.AvailableBalance > 0);
            Assert.NotNull(ofxDocument.Account.Transactions);
            Assert.True(ofxDocument.Account.Transactions.Count > 0);
            var account = (InvestmentAccount)ofxDocument.Account;
            Assert.NotNull(account.StockQuotes);
            Assert.True(account.StockQuotes.Count > 0);
            Console.WriteLine("Test finished");
        }
    }
}
