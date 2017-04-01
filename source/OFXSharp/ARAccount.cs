using System;
using System.Collections.Generic;
using System.Xml;

namespace OFXSharp
{
    public class ARAccount : Account
    {
        public ARAccount(XmlNode node)
        {
            throw new OFXParseException("AR Account type not supported");
        }

        public override void ProcessTransactions(OFXDocumentParser parser, OFXDocument ofx, XmlDocument doc)
        {
        }
    }
}
