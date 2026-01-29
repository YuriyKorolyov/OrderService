using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.DTO.Responses;
using OrderService.WebApi.Clients;
using OrderService.WebApi.Events;
using OrderService.WebApi.Mappers;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.WebApi.UseCases.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private readonly AppDbContext _context;
    private readonly OrderMapper _mapper;
    private readonly IPaymentServiceClient _paymentServiceClient;
    private readonly IOrderEventProducer _orderEventProducer;

    public CreateOrderCommandHandler(
        AppDbContext context,
        OrderMapper mapper,
        IPaymentServiceClient paymentServiceClient,
        IOrderEventProducer orderEventProducer)
    {
        _context = context;
        _mapper = mapper;
        _paymentServiceClient = paymentServiceClient;
        _orderEventProducer = orderEventProducer;
    }

    public async Task<OrderResponse> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var orderEntity = _mapper.ToEntity(command.Request);

        await _context.Orders.AddAsync(orderEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Reserve payment in PaymentService
        var paymentRequest = new PaymentCreateRequest
        {
            OrderId = orderEntity.Id,
            Price = orderEntity.Price
        };

        await _paymentServiceClient.CreatePaymentAsync(paymentRequest);

        // Publish event to Kafka for NotificationService and other consumers
        await _orderEventProducer.PublishOrderCreatedAsync(orderEntity, cancellationToken);

        return _mapper.ToResponse(orderEntity);
    }
}