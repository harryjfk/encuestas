using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;

namespace WebApplication.Controllers
{
    public class AuditoriaController : BaseController<Auditoria>
    {
        public JsonResult Toggle(long id)
        {
            var manager = OwnManager;
            var element = manager.Find(id);
            if (element != null)
            {
               
                manager.Modify(element);
                manager.SaveChanges();
            }
            OwnManager.Get(Query);
            var c = RenderRazorViewToString("_Table", Query);
            var result = new
            {
                Success = true,
                Data = c
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
    }
}
