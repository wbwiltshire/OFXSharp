using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace OFXSharp.XUnit
{
    public class RegressionTests
    {
        [Fact]
        public void OFXBankText()
        {
            string testFile = "santander.ofx";

            var parser = new OFXDocumentParser();
            var ofxDocument = parser.Import(new FileStream(testFile, FileMode.Open));
            Console.WriteLine("Test finished");
        }

        [Fact]
        public void OFXInvestmentText()
        {
            string testFile = "investment.ofx";

            var parser = new OFXDocumentParser();
            var ofxDocument = parser.Import(new FileStream(testFile, FileMode.Open));
            Console.WriteLine("Test finished");
        }
    }
}
