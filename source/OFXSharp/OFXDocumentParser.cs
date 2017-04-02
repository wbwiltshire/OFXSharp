using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Sgml;
using System.Text;

namespace OFXSharp
{
   public class OFXDocumentParser
   {
      public OFXDocument Import(FileStream stream)
      {
         using (var reader = new StreamReader(stream, Encoding.Default))
         {
            return Import(reader.ReadToEnd());
         }
      }

      public OFXDocument Import(string ofx)
      {
         return ParseOfxDocument(ofx);
      }

      private OFXDocument ParseOfxDocument(string ofxString)
      {
         //If OFX file in SGML format, convert to XML
         if (!IsXmlVersion(ofxString))
         {
            ofxString = SGMLToXML(ofxString);
         }

         return Parse(ofxString);
      }

      private OFXDocument Parse(string ofxString)
      {
         OFXDocument ofx = new OFXDocument { AccType = GetAccountType(ofxString) };

         //Load into xml document
         var doc = new XmlDocument();
         doc.Load(new StringReader(ofxString));

         var currencyNode = doc.SelectSingleNode(GetXPath(ofx.AccType, OFXSection.CURRENCY));

         if (currencyNode != null)
         {
            ofx.Currency = currencyNode.FirstChild.Value;
         }
         else
         {
            throw new OFXParseException("Currency not found");
         }

         //Get sign on node from OFX file
         var signOnNode = doc.SelectSingleNode(Resources.SignOn);

         //If exists, populate signon obj, else throw parse error
         if (signOnNode != null)
         {
            ofx.SignOn = new SignOn(signOnNode);
         }
         else
         {
            throw new OFXParseException("Sign On information not found");
         }

         //Get Account information for ofx doc
         var accountNode = doc.SelectSingleNode(GetXPath(ofx.AccType, OFXSection.ACCOUNTINFO));

         //If account info present, populate account object
         if (accountNode != null)
         {
            switch (ofx.AccType)
                {
                    case AccountType.BANK:
                    case AccountType.CC:
                        ofx.Account = new BankAccount(accountNode);
                        //Get list of transactions
                        ofx.Account.ImportTransactions(this, ofx, doc);

                        //Get balance info from ofx doc
                        var ledgerNode = doc.SelectSingleNode(GetXPath(ofx.AccType, OFXSection.BALANCE) + "/LEDGERBAL");
                        var availableNode = doc.SelectSingleNode(GetXPath(ofx.AccType, OFXSection.BALANCE) + "/AVAILBAL");

                        //If balance info present, populate balance object
                        // ***** OFX files from my bank don't have the 'avaliableNode' node, so i manage a 'null' situation
                        if (ledgerNode != null) // && avaliableNode != null
                        {
                            ofx.Balance = new BankBalance(ledgerNode, availableNode);
                        }
                        else
                        {
                            throw new OFXParseException("Balance information not found");
                        }

                        break;
                    case AccountType.AP:
                        ofx.Account = new APAccount(accountNode);
                        break;
                    case AccountType.AR:
                        ofx.Account = new ARAccount(accountNode);
                        break;
                    case AccountType.INVESTMENT:
                        ofx.Account = new InvestmentAccount(accountNode);

                        //Get list of transactions
                        ofx.Account.ImportTransactions(this, ofx, doc);

                        //Get list of positions (requires downcast)
                        ((InvestmentAccount)ofx.Account).ImportPositions(GetXPath(ofx.AccType, OFXDocumentParser.OFXSection.POSITIONS), ofx, doc);

                        //If balance info present, populate balance object
                        var balanceNode = doc.SelectSingleNode(GetXPath(ofx.AccType, OFXSection.BALANCE));
                        if (balanceNode != null)
                        {
                            ofx.Balance = new InvestmentBalance(balanceNode);
                        }
                        else
                        {
                            throw new OFXParseException("Balance information not found");
                        }

                        //Process Stock Quote Prices, if present
                        var SECListNode = doc.SelectSingleNode(Resources.SecurityList);
                        if (SECListNode != null)
                        {
                            //Requires downcast
                            ((InvestmentAccount)ofx.Account).ImportSECList(SECListNode);
                        }
                        break;
                    default:
                        throw new OFXParseException("Invalid Account type.");
                }
         }
         else
         {
            throw new OFXParseException("Account information not found");
         }

         return ofx;
      }

      /// <summary>
      /// Returns the correct xpath to specified section for given account type
      /// </summary>
      /// <param name="type">Account type</param>
      /// <param name="section">Section of OFX document, e.g. Transaction Section</param>
      /// <exception cref="OFXException">Thrown in account type not supported</exception>
      internal string GetXPath(AccountType type, OFXSection section)
      {
         string xpath, accountInfo;

         switch (type)
         {
            case AccountType.BANK:
               xpath = Resources.BankAccount;
               accountInfo = "/BANKACCTFROM";
               break;
            case AccountType.CC:
               xpath = Resources.CCAccount;
               accountInfo = "/CCACCTFROM";
               break;
            case AccountType.INVESTMENT:
                xpath = Resources.Investment;
                accountInfo = "/INVACCTFROM";
                break;
            default:
               throw new OFXException("Account Type not supported. Account type " + type);
         }

         switch (section)
         {
            case OFXSection.ACCOUNTINFO:
               return xpath + accountInfo;
            case OFXSection.BALANCE:
                if (type == AccountType.INVESTMENT)
                    return xpath + "/INVBAL";
                else
                    return xpath;
            case OFXSection.TRANSACTIONS:
                if (type == AccountType.INVESTMENT)
                    return xpath + "/INVTRANLIST";
                else
                    return xpath + "/BANKTRANLIST";
            case OFXSection.POSITIONS:
                return xpath + "/INVPOSLIST";
            case OFXSection.SIGNON:
               return Resources.SignOn;
            case OFXSection.CURRENCY:
               return xpath + "/CURDEF";
            default:
               throw new OFXException("Unknown section found when retrieving XPath. Section " + section);
         }
      }

        /// <summary>
        /// Checks account type of supplied file
        /// </summaryof
        /// <param name="file">OFX file want to check</param>
        /// <returns>Account type for account supplied in ofx file</returns>
        private AccountType GetAccountType(string file)
      {
        if (file.IndexOf("<CREDITCARDMSGSRSV1>") != -1)
            return AccountType.CC;

        if (file.IndexOf("<BANKMSGSRSV1>") != -1)
            return AccountType.BANK;

        if (file.IndexOf("<INVSTMTMSGSRSV1>") != -1)
            return AccountType.INVESTMENT;

        throw new OFXException("Unsupported Account Type");
      }

      /// <summary>
      /// Check if OFX file is in SGML or XML format
      /// </summary>
      /// <param name="file"></param>
      /// <returns></returns>
      private bool IsXmlVersion(string file)
      {
         return (file.IndexOf("OFXHEADER:100") == -1);
      }

      /// <summary>
      /// Converts SGML to XML
      /// </summary>
      /// <param name="file">OFX File (SGML Format)</param>
      /// <returns>OFX File in XML format</returns>
      private string SGMLToXML(string file)
      {
         var reader = new SgmlReader();

         //Inititialize SGML reader
         reader.InputStream = new StringReader(ParseHeader(file));
         reader.DocType = "OFX";

         var sw = new StringWriter();
         var xml = new XmlTextWriter(sw);

         //write output of sgml reader to xml text writer
         while (!reader.EOF)
            xml.WriteNode(reader, true);

         //close xml text writer
         xml.Flush();
         xml.Close();

         var temp = sw.ToString().TrimStart().Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);

         return String.Join("", temp);
      }

      /// <summary>
      /// Checks that the file is supported by checking the header. Removes the header.
      /// </summary>
      /// <param name="file">OFX file</param>
      /// <returns>File, without the header</returns>
      private string ParseHeader(string file)
      {
         //Select header of file and split into array
         //End of header worked out by finding first instance of '<'
         //Array split based of new line & carrige return
         var header = file.Substring(0, file.IndexOf('<'))
            .Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);

         //Check that no errors in header
         CheckHeader(header);

         //Remove header
         return file.Substring(file.IndexOf('<') - 1);
      }

      /// <summary>
      /// Checks that all the elements in the header are supported
      /// </summary>
      /// <param name="header">Header of OFX file in array</param>
      private void CheckHeader(string[] header)
      {
         if (header[0] != "OFXHEADER:100")
            throw new OFXParseException("Incorrect header format");

         if (header[1] != "DATA:OFXSGML")
            throw new OFXParseException("Data type unsupported: " + header[1] + ". OFXSGML required");

         if (header[2] != "VERSION:102")
            throw new OFXParseException("OFX version unsupported. " + header[2]);

         if (header[3] != "SECURITY:NONE")
            throw new OFXParseException("OFX security unsupported");

         if (header[4] != "ENCODING:USASCII")
            throw new OFXParseException("ASCII Format unsupported:" + header[4]);

         if (header[5] != "CHARSET:1252")
            throw new OFXParseException("Charecter set unsupported:" + header[5]);

         if (header[6] != "COMPRESSION:NONE")
            throw new OFXParseException("Compression unsupported");

         if (header[7] != "OLDFILEUID:NONE")
            throw new OFXParseException("OLDFILEUID incorrect");
      }

      #region Nested type: OFXSection

      /// <summary>
      /// Section of OFX Document
      /// </summary>
      internal enum OFXSection
      {
         SIGNON,
         ACCOUNTINFO,
         TRANSACTIONS,
         POSITIONS,
         BALANCE,
         CURRENCY
      }

      #endregion
   }
}