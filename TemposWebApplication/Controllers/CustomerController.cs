using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TemposWebApplication.Controllers
{
    public class CustomerController : Controller
    {
        //
        // GET: /Customer/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Reservation()
        {
            return View();
        }
        
        public ActionResult Loyality()
        {
            return View();
        }
    }
}
