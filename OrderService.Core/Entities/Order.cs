namespace OrderService.Core.Entities;

public class Order
{
    public long Id { get; private set; }
    public long ProductId { get; private set; }
    public int Amount { get; private set; }
    public string EmailClient { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public string Status { get; private set; } = "Created";

    private Order() { }

    public Order(
        long productId,
        int amount,
        string emailClient,
        decimal price,
        string phoneNumber)
    {
        ProductId = productId;
        Amount = amount;
        EmailClient = emailClient;
        Price = price;
        PhoneNumber = phoneNumber;
        CreatedAt = DateTime.UtcNow;
        Status = "Created";
    }

    public void MarkAsPaid() => Status = "Paid";
    public void MarkAsCancelled() => Status = "Cancelled";
}