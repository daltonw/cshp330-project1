using Microsoft.AspNetCore.Mvc;
using MiniCstructor.Business;
using MiniCstructor.Website.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MiniCstructor.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserManager userManager;
        private readonly IClassManager classManager;
        private readonly IEnrollManager enrollManager;
        public HomeController(IUserManager userManager,
                              IClassManager classManager,
                              IEnrollManager enrollManager)
        {
            this.userManager = userManager;
            this.classManager = classManager;
            this.enrollManager = enrollManager;
        }
        public IActionResult Index()
        {
            ViewData["Message"] = "The place for technology classes.";


            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

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
        public ActionResult LogIn()
        {
            ViewData["ReturnUrl"] = Request.Query["returnUrl"];
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(LoginModel loginModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.LogIn(loginModel.UserName, loginModel.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "User name and password do not match.");
                }
                else
                {
                    var json = JsonConvert.SerializeObject(new MiniCstructor.Website.Models.UserModel
                    {
                        Id = user.Id,
                        Name = user.Name
                    });

                    HttpContext.Session.SetString("User", json);

                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, "User"),
                };

                    var claimsIdentity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = false,
                        // Refreshing the authentication session should be allowed.

                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        IsPersistent = false,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        IssuedUtc = DateTimeOffset.UtcNow,
                        // The time at which the authentication ticket was issued.
                    };

                    HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        claimsPrincipal,
                        authProperties).Wait();

                    return Redirect(returnUrl ?? "~/");
                }
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View(loginModel);
        }

        public ActionResult LogOff()
        {
            HttpContext.Session.Remove("User");

            HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("~/");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.Register(registerModel.UserName, registerModel.Password);

                if (user == null)
                {
                    ModelState.AddModelError("msg", "The email is already in use.");
                    return View();
                }


                return Redirect("~/");
            }

            return View();
        }
        public ActionResult ClassList()
        {
            var classes = classManager.Classes
                .Select(t => new MiniCstructor.Website.Models.ClassModel(t.Id, t.Name, t.Description, t.Price))
                .ToArray();
            var model = new ClassListModel { Classes = classes };
            return View(model);
        }

        [Authorize]
        public ActionResult EnrollInClass()
        {
            var classes = classManager.Classes
                .Select(t => new MiniCstructor.Website.Models.ClassModel(t.Id, t.Name, t.Description, t.Price))
                .ToArray();
            var model = new ClassListModel { Classes = classes };

            ViewBag.message = model;
            return View(model);
        }

        [Authorize]
        public ActionResult AddClass(int id)
        {
            var user = JsonConvert.DeserializeObject<Models.UserModel>(HttpContext.Session.GetString("User"));

            if (user == null)
            {
                return RedirectToAction("LogIn");
            }

            enrollManager.Add(user.Id, id);

            return RedirectToAction("StudentClasses");
        }

        [Authorize]
        public ActionResult StudentClasses()
        {
            var userJson = HttpContext.Session.GetString("User");
            var user = JsonConvert.DeserializeObject<Models.UserModel>(userJson);

            var classes = enrollManager.GetAll(user.Id)
                .Select(t => new MiniCstructor.Website.Models.EnrollInClassModel
                {
                    UserId = t.UserId,
                    ClassId = t.ClassId,
                    ClassName = t.ClassName,
                    Description = t.Description,
                    Price = t.Price
                })
                .ToArray();

            return View(classes);
        }


    }
}
