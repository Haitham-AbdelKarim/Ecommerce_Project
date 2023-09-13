using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_Project.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required , MaxLength(200)]
        [Remote("UniqueTitle", "Product", ErrorMessage = "This Name is already exits ",AdditionalFields = "Id")]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }
        [Required]
        public int Price { get; set; }
        [Range(1,UInt32.MaxValue)]
        [Display(Name = "Available Quantity")]
        public int Quantity { get; set; }
        [ForeignKey("Owner")]
        public string OwnerEmail { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public virtual User? Owner { get; set; }
        public string? Image { get; set; }
        public virtual Category? Category { get; set; }

    }
}
