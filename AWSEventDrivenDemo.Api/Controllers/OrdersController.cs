using AWSEventDrivenDemo.Api.DTOs;
using AWSEventDrivenDemo.Domain.Events;
using AWSEventDrivenDemo.Domain.Models;
using AWSEventDrivenDemo.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace AWSEventDrivenDemo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly SnsEventPublisher _eventPublisher;

        public OrdersController(SnsEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = dto.CustomerName,
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                TotalAmount = dto.Items.Sum(i => i.Quantity * i.UnitPrice),
                CreatedAt = DateTime.UtcNow
            };

            var orderEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                CustomerName = order.CustomerName,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                ProductNames = order.Items.Select(x => x.ProductName).ToList()
            };

            await _eventPublisher.PublishOrderCreatedAsync(orderEvent);

            return Ok(new { order.Id });
        }
    }
}
