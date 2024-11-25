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


        [Authorize]
        public IActionResult Assesment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assesment(FileUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid file upload request.";
                return View();
            }

            string fileName = Path.GetFileName(model.File.FileName);
            string fileExtension = Path.GetExtension(fileName)?.ToLowerInvariant();
            string uploadPath = Path.Combine(_environment.WebRootPath, "Uploads");

            try
            {
                // Ensure the upload directory exists
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Validate file
                if (!IsValidFile(model.File, fileExtension, out string validationMessage))
                {
                    ViewBag.ErrorMessage = validationMessage;
                    return View();
                }

                // Save file to the server
                string filePath = Path.Combine(uploadPath, fileName);
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                }

                // Process file
                List<TransactionViewModel> transactions = fileExtension switch
                {
                    ".xml" => await new XmlFileUtilities().ValidateXmlDocumentAsync(model.File, uploadPath),
                    ".csv" => await new CsvFileUtilities().ValidateCsvDocumentAsync(model.File, uploadPath),
                    _ => throw new NotSupportedException($"Unsupported file type: {fileExtension}")
                };

                // Validate data
                if (!IsDataSetValid(transactions, out string dataValidationMessage))
                {
                    ViewBag.ErrorMessage = dataValidationMessage;
                    return View();
                }

                // Save transactions to the database
                var validTransactions = transactions.Select(t => new Transaction
                {
                    TransactionId = t.TransactionId,
                    AccountNumber = t.AccountNumber,
                    Amount = t.Amount,
                    CurrencyCode = t.CurrencyCode,
                    Status = t.Status,
                    TransactionDate = t.TransactionDate
                }).ToList();

                if (validTransactions.Any())
                {
                    _repository.AddTransactions(validTransactions);
                    ViewBag.Message = $"{fileName} uploaded and processed successfully.";
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing file {fileName}: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while processing the file.";
                return View();
            }
        }

        private bool IsValidFile(IFormFile file, string fileExtension, out string validationMessage)
        {
            validationMessage = string.Empty;

            // Validate file type
            var allowedExtensions = new[] { ".xml", ".csv" };
            if (!allowedExtensions.Contains(fileExtension))
            {
                validationMessage = $"Unsupported file type: {fileExtension}. Allowed types are XML and CSV.";
                return false;
            }

            // Validate file size
            const int maxFileSizeMb = 1;
            if (file.Length > maxFileSizeMb * 1024 * 1024)
            {
                validationMessage = $"File size exceeds the maximum limit of {maxFileSizeMb} MB.";
                return false;
            }

            return true;
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

        private bool IsDataSetValid(List<TransactionViewModel> transactions, out string validationMessage)
        {
            validationMessage = string.Empty;

            var invalidFields = new List<string>();

            if (transactions.Any(t => string.IsNullOrWhiteSpace(t.TransactionId)))
                invalidFields.Add("TransactionId");

            if (transactions.Any(t => string.IsNullOrWhiteSpace(t.AccountNumber)))
                invalidFields.Add("AccountNumber");

            if (transactions.Any(t => string.IsNullOrWhiteSpace(t.CurrencyCode)))
                invalidFields.Add("CurrencyCode");

            if (transactions.Any(t => string.IsNullOrWhiteSpace(t.Status)))
                invalidFields.Add("Status");

            if (invalidFields.Any())
            {
                validationMessage = $"The following fields are required: {string.Join(", ", invalidFields)}.";
                return false;
            }

            return true;
        }
    }
}
