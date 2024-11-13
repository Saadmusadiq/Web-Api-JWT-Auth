using JWTAuthenticationForProduct.Models;
using Microsoft.EntityFrameworkCore;

namespace webapic_.Data
{
    public class MyAppDbContext : DbContext
    {
        public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
