using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.DTO.Responses;
using OrderService.WebApi.Mappers;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.WebApi.UseCases.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private readonly AppDbContext _context;
    private readonly OrderMapper _mapper;

    public CreateOrderCommandHandler(AppDbContext context, OrderMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrderResponse> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var orderEntity = _mapper.ToEntity(command.Request);

        await _context.Orders.AddAsync(orderEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.ToResponse(orderEntity);
    }
}