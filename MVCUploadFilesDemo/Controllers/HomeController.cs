using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCUploadFilesDemo.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Uploads()
        { 

            DirectoryInfo salesFTPDirectory = null;
            FileInfo[] files = null;

            try
            {
                string salesFTPPath = (Server.MapPath("~/Uploads/"));
                salesFTPDirectory = new DirectoryInfo(salesFTPPath);
                files = salesFTPDirectory.GetFiles();
            }
            catch (DirectoryNotFoundException exp)
            {
                exp.Message.ToString();
            }
            catch (IOException exp)
            {
                exp.Message.ToString();
            }


            files = files.OrderBy(f => f.Name).ToArray();

            return View(files);
        }      

        [HttpPost]
        public ActionResult UploadFiles()
        {            
                string FileName = "";
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fname;


                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];


                    fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                    file.SaveAs(fname);
                }
                ViewBag.FileStatus = "File uploaded successfully.";
                return Json(FileName, JsonRequestBehavior.AllowGet);                
        }

        [HttpGet]
        public ActionResult Archive()
        {
            string[] files = Directory.GetFiles(
                            Server.MapPath("~/Uploads"));
            List<string> downloads = new List<string>();
            foreach (string file in files)
            {
                downloads.Add(Path.GetFileName(file));
            }
            return View(downloads);
        }

        [HttpPost]
        public ActionResult ZipArchive(List<string> selectedfiles)
        {
            if (System.IO.File.Exists(Server.MapPath
                             ("~/ZipFiles/bundle.zip")))
            {
                System.IO.File.Delete(Server.MapPath
                              ("~/ZipFiles/bundle.zip"));
            }
            ZipArchive zip = ZipFile.Open(Server.MapPath
                     ("~/ZipFiles/bundle.zip"), ZipArchiveMode.Create);
            foreach (string file in selectedfiles)
            {
                zip.CreateEntryFromFile(Server.MapPath
                     ("~/Uploads/" + file), file);
            }
            zip.Dispose();

            return File(Server.MapPath("~/ZipFiles/bundle.zip"),
                      "application/zip", "Archive.zip");

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}