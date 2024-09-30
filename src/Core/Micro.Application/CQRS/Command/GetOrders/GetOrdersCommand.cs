using MediatR;

namespace Micro.Application.CQRS.Command.GetOrders;

public class GetOrdersCommand : IRequest<List<GetOrdersResponse>>
{
    
}