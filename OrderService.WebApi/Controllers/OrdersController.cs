using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.WebApi.DTO.Requests;
using OrderService.WebApi.DTO.Responses;
using OrderService.WebApi.UseCases.Commands;
using OrderService.WebApi.UseCases.Queries;

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

    [HttpGet("{orderId:long}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(long orderId)
    {
        var query = new GetOrderByIdQuery(orderId);
        var result = await _mediator.Send(query);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{orderId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrder(long orderId)
    {
        var command = new DeleteOrderCommand(orderId);
        var deleted = await _mediator.Send(command);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}