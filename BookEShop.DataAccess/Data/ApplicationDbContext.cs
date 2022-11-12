using BookEShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookEShop.DataAccess;

public class ApplicationDbContext : IdentityDbContext
{
    //constructor 
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<CoverType> CoverTypes { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
}
