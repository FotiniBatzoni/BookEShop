using BookEShopWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BookEShopWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        //constructor 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
    }
}
