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
        public async Task<List<TransactionViewModel>> ValidateXmlDocumentAsync(IFormFile file, string path)
        {
            if (file == null) return new List<TransactionViewModel>();

            string fileName = file.FileName;
            string filePath = Path.Combine(path, fileName);

            try
            {
                // Save the file asynchronously to the specified path
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Load and parse the XML file
                string xmlContent = await File.ReadAllTextAsync(filePath);
                var xmlDoc = XDocument.Parse(xmlContent);

                // Extract transactions
                var transactions = xmlDoc
                    .Descendants("Transaction")
                    .Select(e => new TransactionViewModel
                    {
                        TransactionId = e.Attribute("id")?.Value ?? string.Empty,
                        Status = e.Element("Status")?.Value,
                        TransactionDate = ConvertXmlDateTime(e.Element("TransactionDate")?.Value),
                        Amount = decimal.TryParse(e.Element("PaymentDetails")?.Element("Amount")?.Value, out var amount) ? amount : 0,
                        CurrencyCode = e.Element("PaymentDetails")?.Element("CurrencyCode")?.Value ?? string.Empty,
                        AccountNumber = e.Element("PaymentDetails")?.Element("AccountNo")?.Value ?? string.Empty,
                        OutputStatus = GetReturnStatus(e.Element("Status")?.Value)
                    })
                    .ToList();

                return transactions;
            }
            catch (XmlException ex)
            {
                // Log specific XML parsing issues
                Utilities.CreateLog($"XML Parsing Error in file {fileName}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // General exception handling
                Utilities.CreateLog($"Error processing file {fileName}: {ex.Message}");
                throw;
            }
        }

        public string GetReturnStatus(string status) =>
            status switch
            {
                "Approved" => "A",
                "Rejected" => "R",
                "Done" => "D",
                _ => string.Empty
            };

        public DateTime ConvertXmlDateTime(string strDateTime)
        {
            if (DateTime.TryParseExact(strDateTime, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDateTime))
            {
                return parsedDateTime;
            }
            throw new FormatException($"Invalid date format: {strDateTime}");
        }
    }
}
