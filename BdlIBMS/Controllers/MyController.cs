using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdlIBMS.Controllers
{
    public class HomeController : Controller
    {
        // 系统首页
        public ActionResult Index() { return View(); }
    }

    public class UserController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Login() { return View(); }

        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }
    }

    public class ModuleController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }
    }

    public class RoleController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }

        public ActionResult Details() { return View(); }
    }

    public class SystemController : Controller
    {
        public ActionResult BA() { return View(); }

        public ActionResult EnergyMeter() { return View(); }
    }
}