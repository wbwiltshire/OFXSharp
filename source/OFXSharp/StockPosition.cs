using System;
using System.Collections.Generic;
using System.Xml;

namespace OFXSharp
{
    public class StockPosition
    {
        public string UniqueID { get; set; }
        public string UniqueIDType { get; set; }
        public string HeldInAccount { get; set; }
        public decimal Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal MarketValue { get; set; }
        public DateTime AsOfDate { get; set; }
        public string PositionType { get; set; }

        public StockPosition(XmlNode node)
        {
            UniqueID = node.GetValue(".//SECID//UNIQUEID");
            UniqueIDType = node.GetValue(".//SECID//UNIQUEIDTYPE");
            HeldInAccount = node.GetValue(".//HELDINACCT");
            Units = Convert.ToDecimal(node.GetValue(".//UNITS").Trim());
            UnitPrice = Convert.ToDecimal(node.GetValue(".//UNITPRICE").Trim());
            MarketValue = Convert.ToDecimal(node.GetValue(".//MKTVAL").Trim());
            AsOfDate = node.GetValue(".//DTPRICEASOF").ToDate();
            PositionType = node.GetValue("//POSTYPE");
        }
    }
}
