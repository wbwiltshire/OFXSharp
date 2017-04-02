using System;
using System.Collections.Generic;
using System.Xml;

namespace OFXSharp
{
    public class StockInfo
    {
        public string UniqueID { get; set; }
        public string UniqueIDType { get; set; }
        public string SECName { get; set; }
        public string Ticker { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime AsOfDate { get; set; }
        public string StockType { get; set; }

        public StockInfo(XmlNode node)
        {
            UniqueID = node.GetValue(".//SECID//UNIQUEID");
            UniqueIDType = node.GetValue(".//SECID//UNIQUEIDTYPE");
            SECName = node.GetValue(".//SECNAME");
            Ticker = node.GetValue(".//TICKER");
            UnitPrice = Convert.ToDecimal(node.GetValue(".//UNITPRICE").Trim());
            AsOfDate = node.GetValue(".//DTASOF").ToDate();
            StockType = node.GetValue("//STOCKTYPE");
        }
    }
}
