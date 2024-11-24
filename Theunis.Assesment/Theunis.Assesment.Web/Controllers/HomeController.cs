using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using Theunis.Assesment.Web.Models;
using static Theunis.Assesment.Web.Models.XMLTransactionModel;

namespace Theunis.Assesment.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Assesment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assesment(FileUploadViewModel model)
        {
            string fileName = "";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool ValidModel(CSVTransactionModel csvModel)
        {
            bool isValidModel = true;

            foreach (var item in csvModel.Transactions)
            {
                if (item.TransactionIdentifier == null || item.TransactionIdentifier == string.Empty || item.TransactionIdentifier.Length > 50)
                {
                    isValidModel = false;
                    _logger.LogError($"Validation Error for item ID - {item.TransactionIdentifier}. Field - {item.TransactionIdentifier}");
                }
                if (item.Amount == 0)
                {
                    isValidModel = false;
                    _logger.LogError($"Validation Error for item ID - {item.TransactionIdentifier}. Field - {item.Amount}");
                }
                if (!isCurrencyCode(item.CurrencyCode))
                {
                    isValidModel = false;
                    _logger.LogError($"Validation Error for item ID - {item.TransactionIdentifier}. Field - {item.CurrencyCode}");
                }
            }

            return isValidModel;
        }

        private bool isCurrencyCode(string ISOCurrencySymbol)
        {
            var symbol = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(c => !c.IsNeutralCulture).Select(culture =>
            {
                try
                {
                    return new RegionInfo(culture.Name);
                }
                catch
                {
                    return null;
                }
            }).Where(ri => ri != null && ri.ISOCurrencySymbol == ISOCurrencySymbol).Select(ri => ri.CurrencySymbol).FirstOrDefault();

            return symbol != null;
        }

        private bool ValidModel(XmlTransactionsModel xmlModel)
        {
            bool isValidModel = true;

            foreach (var item in xmlModel.Transactions)
            {
                if (item.id == null || item.id.Length > 50)
                {
                    isValidModel = false;
                    _logger.LogError($"Validation Error for item ID - {item.id}. Field - {item.id}");
                }
                if (item.PaymentDetails.Amount == 0)
                {
                    isValidModel = false;
                    _logger.LogError($"Validation Error for item ID - {item.id}. Field - {item.PaymentDetails.Amount}");
                }
                if (!isCurrencyCode(item.PaymentDetails.CurrencyCode))
                {
                    isValidModel = false;
                    _logger.LogError($"Validation Error for item ID - {item.id}. Field - {item.PaymentDetails.CurrencyCode}");
                }
            }

            return isValidModel;
        }

        private string ReadFileAsString(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream());
            return reader.ReadToEnd();
        }

        private CSVTransactionModel ParseCsv(IFormFile file)
        {
            CSVTransactionModel csvModel = new CSVTransactionModel();

            try
            {
                using TextFieldParser parser = new TextFieldParser(file.OpenReadStream());
                csvModel.Transactions = new List<CsvTransaction>();

                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //process row
                    string[] fields = parser.ReadFields();
                    csvModel.Transactions.Add(new CsvTransaction()
                    {
                        TransactionIdentifier = fields[0],
                        Amount = ToInt(fields[1]),
                        CurrencyCode = fields[2],
                        TransactionDate = Convert.ToDateTime(fields[3]),
                        Status = fields[4]
                    });
                }
            }
            catch (Exception e)
            {
                //log
            }
            return csvModel;
        }

        private int ToInt(string num)
        {
            num = num.Replace(",", "");
            int index = num.IndexOf('.');
            num = num.Remove(index);
            return int.Parse(num);
        }

        private T ParseXml<T>(string xml)
        {
            T xmlModel = default(T);

            try
            {
                using TextReader reader = new StringReader(xml);
                xmlModel = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            }
            catch (Exception e)
            {
                //log             
            }
            return xmlModel;
        }

    }
}
