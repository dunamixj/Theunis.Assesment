using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Theunis.Assesment.Web.Models;

namespace Theunis.Assesment.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
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

                    if (fileType == "xml")
                    {

                    }
                    else if(fileType == "csv")
                    {

                    }
                    else
                    {
                        ViewBag.ErrorMessage += string.Format("<b>{0}</b> Unknown format.<br />", fileName);
                        _logger.LogError("Unknown File format");
                        return View();
                    }
                    
                    string wwwPath = this._environment.WebRootPath;
                    string contentPath = this._environment.ContentRootPath;

                    string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    List<string> uploadedFiles = new List<string>();
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        model.File.CopyTo(stream);
                        uploadedFiles.Add(fileName);
                        ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
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

    }
}
