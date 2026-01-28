using MediatR;
using OrderService.WebApi.DTO.Requests;
using OrderService.WebApi.DTO.Responses;

namespace OrderService.WebApi.UseCases.Commands;

public class CreateOrderCommand : IRequest<OrderResponse>
{
    public CreateOrderRequest Request { get; }

    public CreateOrderCommand(CreateOrderRequest request)
    {
        Request = request;
    }
}