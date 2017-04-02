using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace OFXSharp
{
    public class AccountBalance
    {
        public decimal AvailableBalance { get; set; }
    }

    #region BankBalance
    public class BankBalance : AccountBalance
    {
        public decimal LedgerBalance { get; set; }
        public DateTime LedgerBalanceDate { get; set; }
        public DateTime AvailableBalanceDate { get; set; }

        public BankBalance(XmlNode ledgerNode, XmlNode availableNode)
        {
            var tempLedgerBalance = ledgerNode.GetValue("//BALAMT");

            if (!String.IsNullOrEmpty(tempLedgerBalance))
            {
                // ***** Forced Invariant Culture. 
                // If you don't force it, it will use the computer's default (defined in windows control panel, regional settings)
                // So, if the number format of the computer in use it's different from OFX standard (i suppose the english/invariant), 
                // the next line of could crash or (worse) the number would be wrongly interpreted. 
                // For example, my computer has a brazilian regional setting, with "." as thousand separator and "," as 
                // decimal separator, so the value "10.99" (ten 'dollars' (or whatever currency) and ninetynine cents) would be interpreted as "1099" 
                // (one thousand and ninetynine dollars - the "." would be ignored)
                LedgerBalance = Convert.ToDecimal(tempLedgerBalance, CultureInfo.InvariantCulture);
            }
            else
            {
                throw new OFXParseException("Ledger balance has not been set");
            }

            // ***** OFX files from my bank don't have the 'avaliableNode' node, so i manage a null situation
            if (availableNode == null)
            {
                AvailableBalance = 0;

                // ***** this member veriable should be a nullable DateTime, declared as: 
                // public DateTime? LedgerBalanceDate { get; set; }
                // and next line could be:
                // AvaliableBalanceDate = null; 
                AvailableBalanceDate = new DateTime();
            }
            else
            {
                var tempAvailableBalance = availableNode.GetValue("//BALAMT");

                if (!String.IsNullOrEmpty(tempAvailableBalance))
                {
                    // ***** Forced Invariant Culture. (same commment as above)
                    AvailableBalance = Convert.ToDecimal(tempAvailableBalance, CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new OFXParseException("Avaliable balance has not been set");
                }
                AvailableBalanceDate = availableNode.GetValue("//DTASOF").ToDate();
            }

            LedgerBalanceDate = ledgerNode.GetValue("//DTASOF").ToDate();
        }
    }
    #endregion

    #region InvestmentBalance
    public class InvestmentBalance : AccountBalance
    {
        public decimal MarginBalance { get; set; }
        public decimal ShortBalance { get; set; }
        public decimal BuyPower { get; set; }

        public InvestmentBalance(XmlNode balanceNode)
        {
            AvailableBalance = 0;
            var tempAvailable = balanceNode.GetValue(".//AVAILCASH");
            if (!String.IsNullOrEmpty(tempAvailable))
            {
                // ***** Forced Invariant Culture. 
                // If you don't force it, it will use the computer's default (defined in windows control panel, regional settings)
                // So, if the number format of the computer in use it's different from OFX standard (i suppose the english/invariant), 
                // the next line of could crash or (worse) the number would be wrongly interpreted. 
                // For example, my computer has a brazilian regional setting, with "." as thousand separator and "," as 
                // decimal separator, so the value "10.99" (ten 'dollars' (or whatever currency) and ninetynine cents) would be interpreted as "1099" 
                // (one thousand and ninetynine dollars - the "." would be ignored)
                AvailableBalance = Convert.ToDecimal(tempAvailable, CultureInfo.InvariantCulture);
            }
            else
            {
                throw new OFXParseException("Available Cash balance has not been set");
            }

            MarginBalance = 0;
            var tempMargin = balanceNode.GetValue(".//MARGINBALANCE");
            if (!String.IsNullOrEmpty(tempMargin))
            {
                MarginBalance = Convert.ToDecimal(tempMargin, CultureInfo.InvariantCulture);
            }

            ShortBalance = 0;
            var tempShort = balanceNode.GetValue("//SHORTBALANCE");
            if (!String.IsNullOrEmpty(tempShort))
            {
                // ***** Forced Invariant Culture. (same commment as above)
                ShortBalance = Convert.ToDecimal(tempShort, CultureInfo.InvariantCulture);
            }

            BuyPower = 0;
            var tempBuyPower = balanceNode.GetValue("//BUYPOWER");
            if (!String.IsNullOrEmpty(tempBuyPower))
            {
                // ***** Forced Invariant Culture. (same commment as above)
                BuyPower = Convert.ToDecimal(tempBuyPower, CultureInfo.InvariantCulture);
            }

        }
    }
    #endregion
}
