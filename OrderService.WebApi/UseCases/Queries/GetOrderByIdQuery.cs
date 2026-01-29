using MediatR;
using OrderService.WebApi.DTO.Responses;

namespace OrderService.WebApi.UseCases.Queries;

public sealed record GetOrderByIdQuery(long OrderId) : IRequest<OrderResponse?>;

