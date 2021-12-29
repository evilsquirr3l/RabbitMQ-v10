using Microsoft.EntityFrameworkCore;

namespace Subscriber;

public class BeerDbContext : DbContext
{
    public BeerDbContext(DbContextOptions<BeerDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Beer> Beer { get; set; }
}