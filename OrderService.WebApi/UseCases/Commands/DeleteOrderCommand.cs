using MediatR;

namespace OrderService.WebApi.UseCases.Commands;

public sealed record DeleteOrderCommand(long OrderId) : IRequest<bool>;

