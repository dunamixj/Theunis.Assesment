using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Theunis.Assesment.Web.Models;

namespace Theunis.Assesment.Web.Common
{
    public class XmlFileUtilities
    {
        
        public List<TransactionViewModel> ValidateXmlDocument(IFormFile file, string path)
        {
            var fileName = "";
            try
            {
                var lstTrans = new List<TransactionViewModel>();

                if (file != null)
                {
                    fileName = file.FileName;
                    var fileUrl = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(fileUrl, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    var xmlString = File.ReadAllText(fileUrl);
                    
                    XDocument _xmlDoc = XDocument.Load(fileUrl);
                    IEnumerable<XElement> xElements = _xmlDoc.Descendants("Transactions").Elements("Transaction");
                    var q = (from e in xElements
                             select e);

                    lstTrans = (from e in xElements
                                 select new TransactionViewModel
                                 {
                                     TransactionId = e.FirstAttribute.Value,
                                     Status = e.Element("Status")?.Value,
                                     TransactionDate = ConvertXmlDateTime(e.Element("TransactionDate").Value.Trim()),
                                     Amount = decimal.Parse(e.Element("PaymentDetails").Element("Amount").Value),
                                     CurrencyCode = e.Element("PaymentDetails").Element("CurrencyCode").Value,
                                     AccountNumber = e.Element("PaymentDetails").Element("AccountNo").Value,
                                 }).ToList();

                    return lstTrans;
                }
                else
                {
                    return lstTrans;
                }


            }
            catch (Exception e)
            {
                Utilities.CreateLog("CheckXmlFormat : fileName " + fileName + " - " + e);
                throw;
            }

        }

        public DateTime ConvertXmlDateTime(string strDateTime)
        {
            var aaaa = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");

            string format = "yyyy-MM-ddThh:mm:ss";
            DateTime dateTime;
            if (DateTime.TryParseExact(aaaa, format, CultureInfo.CurrentCulture,
                DateTimeStyles.None, out dateTime))
            {
                return dateTime;
            }
            return dateTime;
        }
    }
}
