using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TemposWebApplication.Controllers
{
    public class EmployeeController : Controller
    {
        //
        // GET: /Employee/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Payroll()
        {
            return View();
        }

        public ActionResult Schedule()
        {
            return View();
        }
    }
}
