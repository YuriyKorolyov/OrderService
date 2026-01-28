using System.ComponentModel.DataAnnotations;

namespace OrderService.WebApi.DTO.Requests;

public class CreateOrderRequest
{
    [Required]
    public long ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be at least 1")]
    public int Amount { get; set; }

    [Required]
    [EmailAddress]
    public string EmailClient { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }
}
