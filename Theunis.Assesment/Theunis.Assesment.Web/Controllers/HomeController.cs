using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Diagnostics;
using Theunis.Assesment.Web.Common;
using Theunis.Assesment.Web.Data.Models;
using Theunis.Assesment.Web.Data.Repository.Interfaces;
using Theunis.Assesment.Web.Models;

namespace Theunis.Assesment.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<HomeController> _logger;
        public IAssesmentRepository _repository;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment, IAssesmentRepository repository)
        {
            _logger = logger;
            _environment = environment;
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

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
            try
            {
                if (ModelState.IsValid)
                {
                    var uploadFile = model.File;

                    fileName = Path.GetFileName(model.File.FileName);
                    var fileTypeArr = fileName.Split(".");
                    var fileType = fileTypeArr[1];

                    string wwwPath = this._environment.WebRootPath;
                    string contentPath = this._environment.ContentRootPath;

                    string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    if(CheckFileSize(uploadFile) || CheckFileType(fileType)){
                        List<TransactionViewModel> transactions = new List<TransactionViewModel>();
                        if (fileType == "xml")
                        {
                            XmlFileUtilities xmlUtil = new XmlFileUtilities();
                            transactions = xmlUtil.ValidateXmlDocument(uploadFile, path);
                            
                        }
                        else if (fileType == "csv")
                        {

                        }
                        else
                        {
                            ViewBag.ErrorMessage += string.Format("<b>{0}</b> Unknown format.<br />", fileName);
                            _logger.LogError("Unknown File format");
                            return View();
                        }


                        if (IsDataSetValid(transactions))
                        {
                            var validTransactions = (from t in transactions
                                                     select new Transaction
                                                     {
                                                         AccountNumber = t.AccountNumber,
                                                         Amount = t.Amount,
                                                         CurrencyCode = t.CurrencyCode,
                                                         Status = t.Status,
                                                         TransactionDate = t.TransactionDate,
                                                         TransactionId = t.TransactionId,
                                                     }).ToList();
                            if (validTransactions.Any())
                            {
                                _repository.AddTransactions(validTransactions);
                            }
                            List<string> uploadedFiles = new List<string>();
                            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                            {
                                model.File.CopyTo(stream);
                                uploadedFiles.Add(fileName);
                                ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
                            }

                        }

                        
                    }
                    return View();
                }

                ViewBag.ErrorMessage += string.Format("Please select a file.");

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + string.Format("<b>{0} </b> Unknown format.<br /> ", fileName));
                ViewBag.ErrorMessage += string.Format("<b>{0}</b> Unknown format.<br />", fileName);

            }
            return View();
        }

        // GET: File/ListFiles
        public IActionResult ListFiles()
        {
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
            {
                return View(new List<string>());
            }
            var files = Directory.GetFiles(uploadPath).Select(Path.GetFileName).ToList();
            return View(files);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public bool CheckFileType(string fileType)
        {
            var IsValidFileType = false;

            try
            {
                if (fileType == "xml" || fileType == "csv")
                    return true;
                else
                {
                    ViewBag.ErrorMessage += string.Format("This <b>{0}</b> do not support. <br />", fileType);
                    Utilities.CreateLog(string.Format("This <b>{0}</b> do not support. <br />", fileType));

                    return false;
                }
            }
            catch (Exception e)
            {
                Utilities.CreateLog(e.Message + "CheckFileType");
            }

            return IsValidFileType;
        }

        public bool CheckFileSize(IFormFile file)
        {
            var IsValidTypeSize = false;
            try
            {
                int maxFileSize = 1;
                var fileSize = file.Length;
                if (fileSize > (maxFileSize * 1024 * 1024))
                {
                    ViewBag.ErrorMessage += string.Format("Maximum file size permitted is <b>{0}</b> MB <br />", maxFileSize);
                    Utilities.CreateLog(string.Format("Maximum file size permitted is <b>{0}</b> MB <br />", maxFileSize));
                    return false;
                }

                return true;

            }
            catch (Exception e)
            {
                Utilities.CreateLog(e.Message + "CheckFileSize");
            }

            return IsValidTypeSize;
        }

        public bool IsDataSetValid(List<TransactionViewModel> transactions)
        {
            bool isValid = true;

            var isEmptyTransactionId = (transactions.Where(o => string.IsNullOrEmpty(o.TransactionId))).Count() > 0;
            if (isEmptyTransactionId)
            {
                Utilities.CreateLog("TransactionId is Required");
                isValid = false;
            }
            var isEmptyAccountNumber = (transactions.Where(o => string.IsNullOrEmpty(o.AccountNumber))).Count() > 0;
            if (isEmptyTransactionId)
            {
                Utilities.CreateLog("AccountNumber is Required");
                isValid = false;
            }
            var isEmptyCurrencyCode = (transactions.Where(o => string.IsNullOrEmpty(o.CurrencyCode))).Count() > 0;
            if (isEmptyCurrencyCode)
            {
                Utilities.CreateLog("CurrencyCode is Required");
                isValid = false;
            }
            var isEmptyStatus = (transactions.Where(o => string.IsNullOrEmpty(o.Status))).Count() > 0;
            if (isEmptyStatus)
            {
                Utilities.CreateLog("Status is Required");
                isValid = false;
            }

            return isValid;
        }
    }
}
