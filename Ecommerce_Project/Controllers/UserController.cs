using Ecommerce_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_Project.Controllers
{
    public class UserController : Controller
    {
        EcommerceContext db = new EcommerceContext();
        public string getEmail() => HttpContext.User.FindFirstValue(ClaimTypes.Email);

        [HttpGet]
        public IActionResult SignUp()
        {
            User user = new User();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                db.User.Add(user);
                db.SaveChanges();
                var userIdentity = new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("isAdmin",user.IsAdmin.ToString()),
                    },
                    "cookie"
                );

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync("cookie",claimsPrincipal);
                return RedirectToAction("Index","Home");

            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            User user = new User();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            User? loggedUser = db.User.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            if(loggedUser != null)
            {
                var userIdentity = new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loggedUser.Name),
                        new Claim(ClaimTypes.Email, loggedUser.Email),
                        new Claim("isAdmin",loggedUser.IsAdmin.ToString()),
                    },
                    "cookie"
                );

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync("cookie", claimsPrincipal);
                return RedirectToAction("Index", "Home");
            }
            ViewData["error"] = "Wrong Email or Password";
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("cookie");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ShowProfile()
        {
            User? user = db.User.FirstOrDefault(u => u.Email == getEmail());
            return View(user);
        }

        [HttpGet]
        [Authorize]
        public IActionResult EditProfile()
        {
            User? user = db.User.FirstOrDefault(u => u.Email == getEmail());
            return View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(User user)
        {
            if (ModelState.IsValid)
            {
                User? oldUser = db.User.FirstOrDefault(p => p.Email == user.Email);
                oldUser.Address = user.Address;
                oldUser.Name = user.Name;
                oldUser.Age = user.Age;
                oldUser.Phone = user.Phone;
                oldUser.Password = user.Password;
                var identity = User.Identity as ClaimsIdentity;
                var existingClaim = identity.FindFirst(ClaimTypes.Name);
                if (existingClaim != null)
                    identity.RemoveClaim(existingClaim);

                // add new claim
                identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("cookie", claimsPrincipal);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            return View(user);
        }

        public IActionResult UniqueName(string name)
        {
            User? user = db.User.FirstOrDefault(x => x.Name == name);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
    }
}
