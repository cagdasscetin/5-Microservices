using Micro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Micro.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Stock> Stocks { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>().HasData(new Stock[] {
            new Stock
            {
                Id = 1,
                ProductId = 1,
                Count = 200,
                CreatedDate = DateTime.UtcNow
            },
            new Stock
            {
                Id = 2,
                ProductId = 2,
                Count = 100,
                CreatedDate = DateTime.UtcNow
            },
            new Stock
            {
                Id = 3,
                ProductId = 3,
                Count = 50,
                CreatedDate = DateTime.UtcNow
            },
            new Stock
            {
                Id = 4,
                ProductId = 4,
                Count = 10,
                CreatedDate = DateTime.UtcNow
            },
            new Stock
            {
                Id = 5,
                ProductId = 5,
                Count = 30,
                CreatedDate = DateTime.UtcNow
            },
        });
    }
}