using System.ComponentModel.DataAnnotations;
using Ecommerce_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Project.Validation
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        EcommerceContext db = new EcommerceContext();
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            User? user = db.User.FirstOrDefault(u => u.Email == value.ToString());
            if(user == null)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("This Email is already exist");
            }
        }
    }
}
