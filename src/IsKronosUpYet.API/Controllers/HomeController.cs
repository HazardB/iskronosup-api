using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace IsKronosUpYet.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Ok("API running.");
        }
    }
}
