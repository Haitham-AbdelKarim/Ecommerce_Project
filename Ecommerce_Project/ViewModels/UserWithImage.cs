using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce_Project.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_Project.ViewModels
{
    public class UserWithImage
    {
        [Required]
        [Remote("UniqueName", "User", ErrorMessage = "This Name is already exits ")]
        public string Name { get; set; }
        [Key, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Address { get; set; }
        [Range(21, 60)]
        public int Age { get; set; }
        [StringLength(11)]
        public string Phone { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsAdmin { get; set; } = false;
        public string? image { get; set; } = "default.jpg";
        [Display(Name ="Image")]
        public IFormFile? img { get; set; }
    }
}
