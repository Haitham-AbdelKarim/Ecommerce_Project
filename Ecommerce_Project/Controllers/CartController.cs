using Ecommerce_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ecommerce_Project.Controllers
{
    public class CartController : Controller
    {
        EcommerceContext db = new EcommerceContext();
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddToCart(int Id, int quantity)
        {
            string? email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            Cart? existCart = db.Cart.FirstOrDefault(c => c.ProductId == Id && c.UserEmail == email && c.IsOrdered == false);
            if(existCart != null)
            {
                var product = db.Product.Include(p => p.Category).FirstOrDefault(p => p.Id == Id);
                ViewBag.error = "This product is already exist in your cart";
                return View("../Product/ProductDetails",product);
            }
            Cart newCartItem = new Cart();
            newCartItem.ProductId = Id;
            newCartItem.UserEmail = email;
            newCartItem.Quantity = quantity;
            db.Add(newCartItem);
            db.SaveChanges();
            return RedirectToAction("MyCart");

        }
        [HttpGet]
        [Authorize(policy: "Customer")]
        public IActionResult MyCart()
        {
            string? email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            List<Cart> cart = db.Cart.Include(c => c.Product).Where(c => c.UserEmail == email && c.IsOrdered == false).ToList();
            foreach(var cartItem in cart)
            {
                if(cartItem.Quantity > cartItem.Product.Quantity)
                {
                    cartItem.Quantity = cartItem.Product.Quantity;
                    db.SaveChanges();
                }
            }
            return View(cart);
        }

        [HttpPost]
        [Authorize(policy: "Customer")]
        public IActionResult MyCart(List<Cart> cart,string type)
        {
            foreach(Cart cartItem in cart)
            {
                Cart? oldItem = db.Cart.FirstOrDefault(c => c.Id == cartItem.Id);
                oldItem.Quantity = cartItem.Quantity;
                if(type == "checkout")
                {
                    oldItem.IsOrdered = true;
                    db.SaveChanges();
                    Product? product = db.Product.FirstOrDefault(p => p.Id == cartItem.ProductId);
                    product.Quantity -= cartItem.Quantity;
                    db.SaveChanges();
                }
            }
            
            return RedirectToAction("MyCart");
        }

        [Authorize(policy: "Customer")]
        public IActionResult DeleteCartItem(int id)
        {
            string? email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            Cart? oldItem = db.Cart.FirstOrDefault(c => c.Id == id && c.UserEmail == email);
            if(oldItem != null)
            {
                db.Cart.Remove(oldItem);
                db.SaveChanges();
            }
            return RedirectToAction("MyCart");
        }

        [HttpGet]
        [Authorize(policy: "Customer")]
        public IActionResult MyOrders()
        {
            string? email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            List<Cart> cart = db.Cart.Include(c => c.Product).Where(c => c.UserEmail == email && c.IsOrdered == true).ToList();
            return View(cart);
        }

    }
}
