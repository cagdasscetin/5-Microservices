using Micro.Domain.Enums;

namespace Micro.Domain.Entities;

public class Order : Entity<int>
{
    public int BuyerId { get; set; }
    
    public List<OrderItem> OrderItems { get; set; }
    
    public OrderStatusEnum OrderStatus { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public decimal TotalPrice { get; set; }
}