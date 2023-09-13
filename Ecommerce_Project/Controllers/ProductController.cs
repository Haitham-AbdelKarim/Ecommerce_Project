using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Ecommerce_Project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Project.ViewModels;

namespace Ecommerce_Project.Controllers
{
    public class ProductController : Controller
    {
        EcommerceContext db = new EcommerceContext();
        public string getEmail() => HttpContext.User.FindFirstValue(ClaimTypes.Email);

        private IWebHostEnvironment _hostingEnvironment;

        public ProductController(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }

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
            ProductWithImage product = new ProductWithImage();
            product.OwnerEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            return View(product);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public IActionResult AddProduct(ProductWithImage product)
        {
            if(ModelState.IsValid)
            {
                string fullPath = _hostingEnvironment.WebRootPath + "\\Images\\Product\\";
                Product newProduct = product.toProductAsync().Result;
                db.Product.Add(newProduct);
                db.SaveChanges();
                if (product.ImageFile == null)
                {
                    newProduct.Image = "NoImage.jpg";

                }
                else
                {
                    newProduct.Image = newProduct.Id + Path.GetExtension(product.ImageFile.FileName);
                    using (Stream fileStream = new FileStream(fullPath + newProduct.Image, FileMode.Create))
                    {
                        product.ImageFile.CopyToAsync(fileStream);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("MyProducts");

            }
            return View(product);
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
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
                ProductWithImage productWithImage = new ProductWithImage();
                productWithImage.fromProduct(product);
                return View(productWithImage);
            }
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> EditProductAsync(ProductWithImage productWithImage)
        {
            if (ModelState.IsValid)
            {
                string fullPath = _hostingEnvironment.WebRootPath + "\\Images\\Product\\";
                if ((productWithImage.ImageFile == null && productWithImage.Image != "NoImage.jpg") || (productWithImage.ImageFile != null && productWithImage.Image != "NoImage.jpg"))
                {

                    if (System.IO.File.Exists(fullPath + productWithImage.Image))
                    {
                        System.IO.File.Delete(fullPath + productWithImage.Image);
                    }
                }
                if (productWithImage.ImageFile == null)
                    productWithImage.Image = "NoImage.jpg";
                else
                    productWithImage.Image = productWithImage.Id + Path.GetExtension(productWithImage.ImageFile.FileName);
                Product product = productWithImage.toProductAsync().Result;
                product.Image = productWithImage.Image;
                product.Id = productWithImage.Id;
                product.Category = productWithImage.Category;
                Product? oldProduct = db.Product.FirstOrDefault(p => p.Id == product.Id);
                oldProduct.Title = product.Title;
                oldProduct.CategoryId = product.CategoryId;
                oldProduct.Brand = product.Brand;
                oldProduct.Price = product.Price;
                oldProduct.Quantity = product.Quantity;
                oldProduct.Image = product.Image;
                oldProduct.Description = product.Description;
                if (productWithImage.ImageFile != null)
                {
                    using (Stream fileStream = new FileStream(fullPath + productWithImage.Image, FileMode.Create))
                    {
                        await productWithImage.ImageFile.CopyToAsync(fileStream);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("MyProducts");
            }
            ViewData["Categories"] = db.Category.ToList();
            return View(productWithImage);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public void DeleteProduct(int id)
        {
            Product? product = db.Product.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                if(product.Image != "NotFound.jpg")
                {
                    string fullPath = _hostingEnvironment.WebRootPath + "\\Images\\Product\\";
                    if (System.IO.File.Exists(fullPath + product.Image))
                    {
                        System.IO.File.Delete(fullPath + product.Image);
                    }
                }
                db.Remove(product);
                db.SaveChanges();
            }
            return;
        }

        public IActionResult UniqueTitle(string title,int id)
        {
            Product? product = db.Product.FirstOrDefault(x => x.Title == title);
            if(product == null || product.Id == id)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        public IActionResult ProductDetails(int id)
        {
            var product = db.Product.Include(p=> p.Category).FirstOrDefault(p => p.Id == id);
            return View(product);
        }

        public IActionResult GetProducts(Search? search)
        {
            List<Category> categories = db.Category.ToList();
            Category allCategories = new Category() { Id = -1, Name = "All" };
            categories.Insert(0, allCategories);
            List<Product> products = db.Product.Include(x => x.Category).ToList();
            ViewData["Categories"] = categories;
            if (search == null)
            {
                ViewBag.products = products;
                return View(search);
            }
            else
            {
                if(search.Name != null && search.Name != "")
                {
                    products = products.Where(p => p.Title.ToLower().Contains(search.Name.Trim().ToLower())).ToList();
                }
                if(search.MinPrice >= 0)
                {
                    products = products.Where(p => p.Price >= search.MinPrice).ToList();
                }
                if(search.MaxPrice >= 0)
                {
                    products = products.Where(p => p.Price <= search.MaxPrice).ToList();
                }
                if( search.CategoryId > 0)
                {
                    products = products.Where(p => p.CategoryId == search.CategoryId).ToList();
                }
                ViewBag.products = products;
                return View(search);

            }
            //List<Product>? products = db.Product.Include(x => x.Category).ToList();
        }


    }
}
