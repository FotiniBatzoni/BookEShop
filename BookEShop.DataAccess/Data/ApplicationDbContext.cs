using BookEShop.Models;
using Microsoft.EntityFrameworkCore;

namespace BookEShop.DataAccess;

public class ApplicationDbContext : DbContext
{
    //constructor 
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<CoverType> CoverTypes { get; set; }
}
