using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ecommerce_Project.Models
{
    [PrimaryKey(nameof(Id))]
    public class Cart
    {

        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserEmail { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool IsOrdered { get; set; } = false;
        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}
