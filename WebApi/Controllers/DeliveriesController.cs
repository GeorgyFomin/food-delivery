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
            var createDeliveryId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetDelivery), new { id = createDeliveryId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDelivery(int id, [FromBody] EditDelivery.Command command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(command));
        }
        [HttpDelete("{id}")]
        //[HttpPut("{id}")]
        public async Task<ActionResult> DeleteDelivery(int id)
        {
            await _mediator.Send(new DeleteDelivery.Command { Id = id });
            return NoContent();
        }
    }
}
