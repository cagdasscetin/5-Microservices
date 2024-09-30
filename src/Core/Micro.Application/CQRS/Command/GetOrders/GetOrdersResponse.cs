using Micro.Domain.Enums;

namespace Micro.Application.CQRS.Command.GetOrders;

public class GetOrdersResponse
{
    public int BuyerId { get; set; }
    
    public List<OrderItemResponse> OrderItems { get; set; }
    
    public OrderStatusEnum OrderStatus { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public decimal TotalPrice { get; set; }
}

public class OrderItemResponse
{
    public int ProductId { get; set; }
    
    public int Count { get; set; }
    
    public decimal Price { get; set; }
}