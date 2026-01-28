using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.WebApi.DTO.Requests;
using OrderService.WebApi.DTO.Responses;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var command = new CreateOrderCommand(request);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}