using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Yare.Models;

namespace Yare.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Watch> Watches { get; set; }
        public DbSet<Jewellery> Jewellerys { get; set; }
        public DbSet<Accessory> Accessories { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Product_Collection> Product_Collections { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }    
        public DbSet<OrderDetail> OrderDetails { get; set; }

    }
}
