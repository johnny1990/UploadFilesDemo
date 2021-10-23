using MVCUploadFilesDemo.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Ionic.Zip;


namespace MVCUploadFilesDemo.Controllers
{
    public class ArchiveAPIController : ApiController
    {
        AchiveDBEntities db = new AchiveDBEntities();

        //web api service endpoint necessary to archive files and store operations in database
        [HttpPost]
        [Route("api/ArchiveAPI/DownloadZipArchive")]
        public HttpResponseMessage DownloadZipArchive(List<FilesArchiveModel> files)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            //start timer to count archiving time
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Operation op = new Operation();

            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                //create folder in Archive for files
                zip.AddDirectoryByName("ZipFiles");


                foreach (FilesArchiveModel file in files)
                {
                    if (file.IsSelected)
                    {   //add each file to folder 
                        zip.AddFile(file.FilePath, "ZipFiles");
                    }
                }

                //zip file name
                string zipName = String.Format("ArchiveFilesZipArchive" + "_" + DateTime.Now.ToString("MM/dd/yyyy h:mm:ss", CultureInfo.InvariantCulture) + ".zip");


                using (MemoryStream memoryStream = new MemoryStream())
                {
                    //save zip file
                    zip.Save(memoryStream);

                    #region Save into database
                    //stop execution time archiving
                    watch.Stop();
                    //count time in seconds
                    var elapsedTimeMiliseconds = watch.ElapsedMilliseconds;

                    //Save to database operations
                    if (ModelState.IsValid)//archive success 
                    {
                        op.Name = "ArchiveFilesZipArchive" + "_" + DateTime.Now.ToString("MM/dd/yyyy h:mm:ss", CultureInfo.InvariantCulture) + ".zip";
                        op.Date = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy h:mm:ss", CultureInfo.InvariantCulture));
                        op.ArchivingTime = elapsedTimeMiliseconds;
                        op.Status = "Succes";
                        db.Operations.Add(op);
                        db.SaveChanges();
                    }
                    else//archive error
                    {
                        op.Name = "ArchiveFilesZipArchive" + "_" + DateTime.Now.ToString("MM/dd/yyyy h:mm:ss", CultureInfo.InvariantCulture) + ".zip";
                        op.Date = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy h:mm:ss", CultureInfo.InvariantCulture));
                        op.ArchivingTime = elapsedTimeMiliseconds;
                        op.Status = "Error";
                        db.Operations.Add(op);
                        db.SaveChanges();
                    }
                    #endregion

                    // set content
                    response.Content = new ByteArrayContent(memoryStream.ToArray());
                    response.Content.Headers.ContentLength = memoryStream.ToArray().LongLength;

                    //Set the Content Disposition http Header
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = zipName;

                    //configuring content tipe as zip.
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");

                    return response;
                }
            }
        }
    }
}
