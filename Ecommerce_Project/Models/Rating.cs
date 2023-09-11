using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ecommerce_Project.Models
{
    [PrimaryKey(nameof(UserEmail), nameof(ProductId))]
    public class Rating
    {
        [ForeignKey("User")]
        public string UserEmail { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Rate { get; set; }

        public virtual User User { get; set; }
        public virtual Product Product { get; set; }

    }
}
