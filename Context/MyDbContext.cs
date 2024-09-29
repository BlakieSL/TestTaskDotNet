using Microsoft.EntityFrameworkCore;
using TestTask.Model;

namespace TestTask.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext() { } 
    
    public MyDbContext(DbContextOptions options) : base(options){ }
    
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);
    }
}