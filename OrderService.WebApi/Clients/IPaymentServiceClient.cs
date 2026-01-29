using Refit;

namespace OrderService.WebApi.Clients;

public interface IPaymentServiceClient
{
    [Post("/api/payments/create")]
    Task<ApiResponse<object>> CreatePaymentAsync([Body] PaymentCreateRequest request);
}

public sealed class PaymentCreateRequest
{
    public long OrderId { get; set; }
    public decimal Price { get; set; }
}

