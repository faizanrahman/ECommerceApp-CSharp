using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ECommerceApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetString("loggedin") == "true")
            {
                int? userId = HttpContext.Session.GetInt32("ID");
                String userName = HttpContext.Session.GetString("Name");

                ViewBag.userName = userName;
                ViewBag.userId = userId;


                return View();
            }
            return RedirectToAction("Index", "Auth");
        }
    }
}
