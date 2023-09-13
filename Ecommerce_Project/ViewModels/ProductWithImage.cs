using Ecommerce_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_Project.ViewModels
{
    public class ProductWithImage
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        [Remote("UniqueTitle", "Product", ErrorMessage = "This Name is already exits ",AdditionalFields = "Id")]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }
        [Required]
        public int Price { get; set; }
        [Range(1, UInt32.MaxValue)]
        public int Quantity { get; set; }
        [ForeignKey("Owner")]
        public string OwnerEmail { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public virtual User? Owner { get; set; }
        [Display(Name = "Image")]
        public string? Image { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<Rating>? Rating { get; set; }

        public async Task<Product> toProductAsync()
        {
            Product product = new Product();
            product.Title = Title;
            product.Description = Description;
            product.Brand = Brand;
            product.Price = Price;
            product.Quantity = Quantity;
            product.OwnerEmail = OwnerEmail;
            product.CategoryId = CategoryId;
            return product;

        }

        public void fromProduct(Product product)
        {
            Title = product.Title;
            Description = product.Description;
            Brand = product.Brand;
            Price = product.Price;
            Quantity = product.Quantity;
            OwnerEmail = product.OwnerEmail;
            CategoryId = product.CategoryId;
            Image = product.Image;
            Id = product.Id;
            Category = product.Category;
            return;

        }
    }
}
