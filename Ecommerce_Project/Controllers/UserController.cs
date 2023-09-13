using Ecommerce_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Project.ViewModels;

namespace Ecommerce_Project.Controllers
{
    public class UserController : Controller
    {
        EcommerceContext db = new EcommerceContext();

        private IWebHostEnvironment _hostingEnvironment;

        public UserController(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }

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
                try
                {
                    db.User.Add(user);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.emailError = "This Email is already exist";
                    return View(user);
                }
                ;
                var userIdentity = new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("isAdmin",user.IsAdmin.ToString()),
                        new Claim("image", user.image),
                    },
                    "cookie"
                ); ;

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
            //System.Web.HttpContext.Current.Server.MapPath(path);
            //System.Web.Hosting.HostingEnvironment.MapPath(path);
            User? loggedUser = db.User.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            if(loggedUser != null)
            {
                var userIdentity = new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loggedUser.Name),
                        new Claim(ClaimTypes.Email, loggedUser.Email),
                        new Claim("isAdmin",loggedUser.IsAdmin.ToString()),
                        new Claim("image", loggedUser.image),
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
            User? oldUser = db.User.FirstOrDefault(u => u.Email == getEmail());
            UserWithImage user = new UserWithImage();
            user.Email = oldUser.Email;
            user.Address = oldUser.Address;
            user.Phone = oldUser.Phone;
            user.Age = oldUser.Age;
            user.image = oldUser.image;
            user.Name = oldUser.Name;
            user.IsAdmin = oldUser.IsAdmin;
            return View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(UserWithImage user)
        {
            var splitImage = user.image.Split('.');
            string fullPath = _hostingEnvironment.WebRootPath + "\\Images\\User\\";
            if ((user.img == null && user.image != "default.jpg") || (user.img != null && user.image != "default.jpg"))
            {

                if (System.IO.File.Exists(fullPath + user.image))
                {
                    System.IO.File.Delete(fullPath + user.image);
                }
            }
            if (user.img == null)
                user.image = "default.jpg";
            else
                user.image = user.Email + Path.GetExtension(user.img.FileName);

            if (ModelState.IsValid)
            {
                User? oldUser = db.User.FirstOrDefault(p => p.Email == user.Email);
                oldUser.Address = user.Address;
                oldUser.Name = user.Name;
                oldUser.Age = user.Age;
                oldUser.Phone = user.Phone;
                oldUser.Password = user.Password;
                oldUser.image = user.image;
                if (user.img != null)
                {
                    using (Stream fileStream = new FileStream(fullPath + user.image, FileMode.Create))
                    {
                        await user.img.CopyToAsync(fileStream);
                    }
                }
                var identity = User.Identity as ClaimsIdentity;
                var existingClaimName = identity.FindFirst(ClaimTypes.Name);
                if (existingClaimName != null)
                    identity.RemoveClaim(existingClaimName);

                var existingClaimImage = identity.FindFirst("image");
                if (existingClaimImage != null)
                    identity.RemoveClaim(existingClaimImage);

                // add new claim
                identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                identity.AddClaim(new Claim("image", user.image));
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
