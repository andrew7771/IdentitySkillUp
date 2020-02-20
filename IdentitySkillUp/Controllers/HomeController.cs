using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentitySkillUp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace IdentitySkillUp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<PluralsightUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<PluralsightUser> _claimsPrincipalFactory;
        private readonly SignInManager<PluralsightUser> _signInManager;

        public HomeController(UserManager<PluralsightUser> userManager,
            IUserClaimsPrincipalFactory<PluralsightUser> claimsPrincipalFactory,
            SignInManager<PluralsightUser> signInManager)
        {
            _userManager = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    user = new PluralsightUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.UserName
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Errors.Any())
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(error.Code, error.Description);
                        }
                        return View();
                    }
                }

                return View("Success");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                var signInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

                if (signInResult.Succeeded)
                    return RedirectToAction("Index");

                ModelState.AddModelError("", "Invalid UserName or Password");
            }
             
            return View();
        }
    }
}
