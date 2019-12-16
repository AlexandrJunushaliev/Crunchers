using System;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace UploadingFilesUsingMVC.Controllers
{
    public class FileUploadController : Controller
    {
        // GET: FileUpload    
        public ActionResult Index(int categoryId)
        {
            ViewBag.categoryId = categoryId;
            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase file, int categoryId)
        {
            ViewBag.categoryId = categoryId;
            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null)
                    {
                        string name = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);

                        if (!Directory.Exists(Server.MapPath("~/UploadedFiles")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/UploadedFiles"));
                        }

                        string path = Path.Combine(Server.MapPath("~/UploadedFiles"), name + extension);
                        FileInfo info = new FileInfo(path);
                        for (int i = 0; i < int.MaxValue; i++)
                        {
                            if (!info.Exists)
                            {
                                break;
                            }

                            path = Path.Combine(Server.MapPath("~/UploadedFiles"), name + i + extension);
                            info = new FileInfo(path);
                        }

                        file.SaveAs(path);

                        string link =
                            $"{Request.Url.Scheme}://{Request.Url.Authority}{Url.Content("~")}files?fileName={info.Name}";
                        ViewBag.link = link;
                        ViewBag.fileStatus = $"Файл был загружен успешно. Ссылка: {link}";
                    }
                }
                catch (Exception)
                {
                    ViewBag.fileStatus = "Error while file uploading.";
                }
            }

            return View("Index");
        }
    }
}