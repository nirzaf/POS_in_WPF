using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TemposWebApplication.Controllers
{
    public class MenuController : Controller
    {
        //
        // GET: /Menu/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ItemDetails()
        {
            return View();
        }

        public ActionResult Items()
        {
            return View();
        }

    }
}
