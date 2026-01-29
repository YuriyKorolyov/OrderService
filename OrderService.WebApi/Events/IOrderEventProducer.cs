using OrderService.DataAccess.Postgres.Models;

namespace OrderService.WebApi.Events;

public interface IOrderEventProducer
{
    Task PublishOrderCreatedAsync(Order order, CancellationToken cancellationToken = default);
}

