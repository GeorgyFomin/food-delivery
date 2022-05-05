using Microsoft.AspNetCore.Mvc;
using MediatR;
using UseCases.API.OrderItems;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderItemsController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<OrderItemDto>> GetOrderItems() => await _mediator.Send(new GetOrderItems.Query());
        [HttpGet("{id}")]
        public async Task<OrderItemDto?> GetOrderItem(int id) => await _mediator.Send(new GetOrderItemById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateOrderItem(OrderItemDto orderItemDto)
        {
            if (orderItemDto == null)
            {
                throw new EntityNotFoundException("OrderItemDto not found");
            }
            int createOrderItemId = await _mediator.Send(new AddOrderItem.Command
            {
                Product = orderItemDto.Product,
                Quantity = orderItemDto.Quantity
            });
            return CreatedAtAction(nameof(GetOrderItem), new { id = createOrderItemId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(int id, OrderItemDto orderItemDto)
        {
            if (id != orderItemDto.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(new EditOrderItem.Command()
            {
                Id = orderItemDto.Id,
                Product = orderItemDto.Product,
                Quantity = orderItemDto.Quantity
            }));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrderItem(int id)
        {
            await _mediator.Send(new DeleteOrderItem.Command { Id = id });
            return NoContent();
        }
    }
}
