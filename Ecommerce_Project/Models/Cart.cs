using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ecommerce_Project.Models
{
    [PrimaryKey(nameof(UserEmail), nameof(ProductId))]
    public class Cart
    {
        [ForeignKey("User")]
        public string UserEmail { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsOrdered { get; set; } = false;
        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}
