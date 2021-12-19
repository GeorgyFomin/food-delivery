using Microsoft.AspNetCore.Mvc;
using MediatR;
using Entities;
using UseCases.API.Deliveries;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : Controller
    {
        private readonly IMediator _mediator;
        public DeliveriesController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<Delivery>> GetDeliveries() => await _mediator.Send(new GetDeliveries.Query());
        [HttpGet("{id}")]
        public async Task<Delivery> GetDelivery(int id) => await _mediator.Send(new GetDeliveryById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateDelivery([FromBody] AddDelivery.Command command)
        {
            int createDeliveryId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetDelivery), new { id = createDeliveryId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDelivery(int id, Delivery delivery)
        {
            if (id != delivery.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(
                new EditDelivery.Command { Id = delivery.Id, Price = delivery.Price, ServiceName = delivery.ServiceName, TimeSpan = delivery.TimeSpan }));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDelivery(int id)
        {
            await _mediator.Send(new DeleteDelivery.Command { Id = id });
            return NoContent();
        }
    }
}
