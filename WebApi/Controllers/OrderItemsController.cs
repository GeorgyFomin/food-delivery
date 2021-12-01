using Microsoft.AspNetCore.Mvc;
using MediatR;
using Entities;
using UseCases.API.OrderItems;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController:ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderItemsController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<OrderItem>> GetOrderItems() => await _mediator.Send(new GetOrderItems.Query());
        [HttpGet("{id}")]
        public async Task<OrderItem> GetOrderItem(int id) => await _mediator.Send(new GetOrderItemById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateOrderItem([FromBody] AddOrderItem.Command command)
        {
            var createOrderItemId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrderItem), new { id = createOrderItemId }, null);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteOrderItem(int id)
        {
            await _mediator.Send(new DeleteOrderItem.Command { Id = id });
            return NoContent();
        }
    }
}
