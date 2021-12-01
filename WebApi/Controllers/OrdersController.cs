using Microsoft.AspNetCore.Mvc;
using MediatR;
using Entities;
using UseCases.API.Orders;
namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController:ControllerBase
    {
        private readonly IMediator _mediator;
        public OrdersController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<Order>> GetOrders() => await _mediator.Send(new GetOrders.Query());
        [HttpGet("{id}")]
        public async Task<Order> GetOrder(int id) => await _mediator.Send(new GetOrderById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody] AddOrder.Command command)
        {
            var createOrderId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrder), new { id = createOrderId }, null);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            await _mediator.Send(new DeleteOrder.Command { Id = id });
            return NoContent();
        }
    }
}
