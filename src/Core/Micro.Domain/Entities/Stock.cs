namespace Micro.Domain.Entities;

public class Stock : Entity<int>
{
    public int ProductId { get; set; }
    
    public int Count { get; set; }
    
    public DateTime CreatedDate { get; set; }
}