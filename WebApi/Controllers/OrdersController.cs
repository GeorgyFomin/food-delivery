using Microsoft.AspNetCore.Mvc;
using MediatR;
using UseCases.API.Orders;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrdersController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<OrderDto>> GetOrders() => await _mediator.Send(new GetOrders.Query());
        [HttpGet("{id}")]
        public async Task<OrderDto?> GetOrder(int id) => await _mediator.Send(new GetOrderById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateOrder(OrderDto orderDto)
        {
            if (orderDto == null)
            {
                throw new EntityNotFoundException("OrderDto not found");
            }
            int createOrderId = await _mediator.Send(new AddOrder.Command
            {
                Delivery = orderDto.Delivery,
                Discount = orderDto.Discount,
                OrderElements = orderDto.OrderElements,
                PhoneNumber = orderDto.PhoneNumber < 10 ? 10 : orderDto.PhoneNumber
            });
            return CreatedAtAction(nameof(GetOrder), new { id = createOrderId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderDto orderDto)
        {
            if (id != orderDto.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(new EditOrder.Command()
            {
                Id = orderDto.Id,
                OrderElements = orderDto.OrderElements,
                PhoneNumber = orderDto.PhoneNumber < 10 ? 10 : orderDto.PhoneNumber,
                Delivery = orderDto.Delivery,
                Discount = orderDto.Discount
            }));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            await _mediator.Send(new DeleteOrder.Command { Id = id });
            return NoContent();
        }
    }
}
