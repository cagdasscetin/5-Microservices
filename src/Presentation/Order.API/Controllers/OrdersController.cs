using MediatR;
using Micro.Application.CQRS.Command.CreateOrder;
using Micro.Application.CQRS.Command.GetOrders;
using Microsoft.AspNetCore.Mvc;

namespace Order.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : Controller
{
    private readonly IMediator _mediator;
    
    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCommand request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var response = await _mediator.Send(new GetOrdersCommand());

        return Ok(response);
    }
}