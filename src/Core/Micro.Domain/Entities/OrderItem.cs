namespace Micro.Domain.Entities;

public class OrderItem : Entity<int>
{
    public int ProductId { get; set; }
    
    public int Count { get; set; }
    
    public decimal Price { get; set; }
    
    public int OrderId { get; set; }
}