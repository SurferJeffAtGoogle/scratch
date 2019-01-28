// Copyright (c) 2019 Google LLC.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

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
