using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MvcApp.Models;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    /// <summary>
    /// Trivial controller for returning the count of files in a particular 
    /// </summary>
    public class BlobStorageController : Controller
    {
        private const string BlobServiceClientUrlSettingName = "BlobServiceClientUrl";
        private const string BlobContainerNameSettingName = "BlobContainerName";
        private readonly string BlobServiceUrl;
        private readonly string BlobContainerName;

        public BlobStorageController()
        {
            BlobServiceUrl = ConfigurationManager.AppSettings[BlobServiceClientUrlSettingName];
            BlobContainerName = ConfigurationManager.AppSettings[BlobContainerNameSettingName];
        }

        public async Task<ActionResult> Index()
        {
            // DefaultAzureCredential will look for Azure identity from Visual Studio or environment
            // variables when run locally, and from a managed identity or environment variables when
            // run in the cloud.
            var client = new BlobServiceClient(new Uri(BlobServiceUrl), new DefaultAzureCredential())
                .GetBlobContainerClient(BlobContainerName);

            var fileCount = 0;
            var blobEnumerator = client.GetBlobsAsync().GetAsyncEnumerator();
            while (await blobEnumerator.MoveNextAsync())
            {
                fileCount++;
            }

            return View(new ShareContents { Path = $"{BlobServiceUrl}/{BlobContainerName}", FileCount = fileCount });
        }

        public ActionResult AddFile() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddFile(NewFileRequest request)
        {
            // DefaultAzureCredential will look for Azure identity from Visual Studio or environment
            // variables when run locally, and from a managed identity or environment variables when
            // run in the cloud.
            var client = new BlobServiceClient(new Uri(BlobServiceUrl), new DefaultAzureCredential())
                .GetBlobContainerClient(BlobContainerName)
                .GetBlobClient(request.FileName);

            await client.UploadAsync(BinaryData.FromString(request.Content), true);

            return RedirectToAction("Index");
        }
    }
}