using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Ecommerce_Project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_Project.Controllers
{
    public class ProductController : Controller
    {
        EcommerceContext db = new EcommerceContext();
        public string getEmail() => HttpContext.User.FindFirstValue(ClaimTypes.Email);

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public IActionResult MyProducts()
        {
            string email = getEmail();
            List<Product>? products = db.User.Include(x => x.Products).FirstOrDefault(x => x.Email == email)?.Products?.ToList();
            products?.ForEach(x => x.Category = db.Category.FirstOrDefault(c => c.Id == x.CategoryId));
            return View(products);
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public IActionResult AddProduct()
        {
            ViewData["Categories"] = db.Category.ToList();
            Product product = new Product();
            product.OwnerEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            return View(product);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public IActionResult AddProduct(Product product)
        {
            if(ModelState.IsValid)
            {
                db.Product.Add(product);
                db.SaveChanges();
                return RedirectToAction("MyProducts");

            }
            return View(product);
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            Product? product = db.Product.FirstOrDefault(p => p.Id == id);
            if(product == null || product?.OwnerEmail != getEmail())
            {
                return View("NotFound");
            }
            else
            {
                ViewData["Categories"] = db.Category.ToList();
                return View(product);
            }
        }

        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                Product? oldProduct = db.Product.FirstOrDefault(p => p.Id == product.Id);
                oldProduct.Title = product.Title;
                oldProduct.CategoryId = product.CategoryId;
                oldProduct.Brand = product.Brand;
                oldProduct.Price = product.Price;
                oldProduct.Quantity = product.Quantity;
                db.SaveChanges();
                return RedirectToAction("MyProducts");
            }
            ViewData["Categories"] = db.Category.ToList();
            return View(product);
        }

        [HttpPost]
        public void DeleteProduct(int id)
        {
            Product? product = db.Product.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                db.Remove(product);
                db.SaveChanges();
            }
            return;
        }

    }
}
