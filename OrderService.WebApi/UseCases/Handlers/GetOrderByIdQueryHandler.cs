using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres;
using OrderService.WebApi.DTO.Responses;
using OrderService.WebApi.Mappers;
using OrderService.WebApi.UseCases.Queries;

namespace OrderService.WebApi.UseCases.Handlers;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse?>
{
    private readonly AppDbContext _context;
    private readonly OrderMapper _mapper;

    public GetOrderByIdQueryHandler(AppDbContext context, OrderMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrderResponse?> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return null;
        }

        return _mapper.ToResponse(order);
    }
}

