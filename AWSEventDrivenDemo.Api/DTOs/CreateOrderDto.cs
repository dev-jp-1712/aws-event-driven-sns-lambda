using System.ComponentModel.DataAnnotations;

namespace AWSEventDrivenDemo.Api.DTOs
{
    public class CreateOrderItemDto
    {
        [Required]
        public string ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }

    public class CreateOrderDto
    {
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
