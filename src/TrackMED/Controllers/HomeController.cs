using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TrackMED.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Brief description of the system's capabilities";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "How to contact system creator.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
