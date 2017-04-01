using System;
using System.Collections.Generic;
using System.Xml;

namespace OFXSharp
{
    public class APAccount : Account
    {
        public APAccount(XmlNode node)
        {
            throw new OFXParseException("AP Account type not supported");
        }
        public override void ProcessTransactions(OFXDocumentParser parser, OFXDocument ofx, XmlDocument doc)
        {
        }
    }
}
