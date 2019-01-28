using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GuessWord.Models;
using GuessWord.ViewModels;
using Microsoft.Extensions.Options;

namespace GuessWord.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<SecretWordOptions> _secretWordOptions;

        public HomeController(IOptions<SecretWordOptions> secretWordOptions)
        {
            _secretWordOptions = secretWordOptions;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(GuessModel model)
        {
            if (ModelState.IsValid) 
            {
                bool itsRight = _secretWordOptions.Value.SecretWord == model.Word;
                ViewData["guess"] = itsRight ? "right" : "wrong";
            }
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "This is the best application ever written.";

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
