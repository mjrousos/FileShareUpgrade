using MvcApp.Models;
using System.Configuration;
using System.IO;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    /// <summary>
    /// Trivial controller for returning the count of files in a particular 
    /// </summary>
    public class HomeController : Controller
    {
        private const string FileSharePathSettingName = "FileSharePath";
        private readonly string FilePath;

        public HomeController()
        {
            FilePath = ConfigurationManager.AppSettings[FileSharePathSettingName];
        }

        public ActionResult Index()
        {
            var fileCount = Directory.GetFiles(FilePath, "*", SearchOption.AllDirectories).Length;
            return View(new ShareContents { Path = FilePath, FileCount = fileCount });
        }

        public ActionResult AddFile() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFile(NewFileRequest request)
        {
            var path = Path.Combine(FilePath, request.FileName);
            System.IO.File.WriteAllText(path, request.Content);

            return RedirectToAction("Index");
        }
    }
}