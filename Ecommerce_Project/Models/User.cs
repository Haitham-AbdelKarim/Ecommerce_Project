using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce_Project.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_Project.Models
{
    public class User
    {
        [Required]
        [Remote("UniqueName", "User", ErrorMessage = "This Name is already exits ")]
        public string Name { get; set; }
        [Key, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Address { get; set; }
        [Range(21,60)]
        public int Age { get; set; }
        [RegularExpression("^\\d{11,11}$", ErrorMessage = "Phone should contain Numbers only with lenght 11")]
        public string Phone { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsAdmin { get; set; } = false;
        public string? image { get; set; } = "default.jpg";
        public virtual ICollection<Cart>? Cart { get; set;}
        public virtual ICollection<Product>? Products { get;}
    }
}
