using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace Ecommerce_Project.Models
{
    public class EcommerceContext : DbContext
    {
        private string _connString;
        public EcommerceContext()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
            IConfiguration configuration = builder.Build();
            _connString = configuration.GetValue<string>("ConnectionStrings:value");
        }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connString);
        }


    }
}
