using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class DownloadController : Controller
    {
        public FileResult ManualAdministrativo()
        {
            string virtualFilePath = Server.MapPath("~/Static/Pdf/ManualAdministrativo.pdf");
            var fileStream = new FileStream(virtualFilePath, FileMode.Open, FileAccess.Read);
            var fsResult = new FileStreamResult(fileStream, "application/pdf");
            return fsResult;
            //string virtualFilePath = Server.MapPath("~/App_Data/Pdf/ManualAdministrativo.pdf");
            //return File(virtualFilePath, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(virtualFilePath));
        }  
	}
}