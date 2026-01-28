using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.DTO.Requests;
using OrderService.WebApi.DTO.Responses;
using Riok.Mapperly.Abstractions;

namespace OrderService.WebApi.Mappers;

[Mapper]
public partial class OrderMapper
{
    public partial Order ToEntity(CreateOrderRequest request);

    public partial OrderResponse ToResponse(Order order);

    [MapProperty(nameof(Order.CreatedAt), nameof(OrderResponse.CreatedAt))]
    [MapProperty(nameof(Order.Status), nameof(OrderResponse.Status))]
    public partial OrderResponse ToResponseWithCustomMapping(Order order);

    public partial void UpdateEntityFromRequest(
        CreateOrderRequest request,
        Order order);
}