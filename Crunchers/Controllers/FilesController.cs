using System;
using System.IO;
using System.Web.Mvc;

namespace Crunchers.Controllers
{
    public class FilesController : Controller
    {
        // GET
        public FileResult Index(string fileName)
        {
            string filePath = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return null;
            }

            var reg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Path.GetExtension(filePath).ToLower());
            string contentType = "application/unknown";

            if (reg != null)
            {
                string registryContentType = reg.GetValue("Content Type") as string;

                if (!String.IsNullOrWhiteSpace(registryContentType))
                {
                    contentType = registryContentType;
                }
            }

            return File(filePath, contentType, fileName);
        }
    }
}