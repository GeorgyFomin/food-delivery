using Microsoft.AspNetCore.Mvc;
using MediatR;
using Entities.Domain;
using UseCases.API.Deliveries;
using UseCases.API.Deliveries.Dto;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : Controller
    {
        private readonly IMediator _mediator;
        public DeliveriesController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<DeliveryDto>> GetDeliveries() => await _mediator.Send(new GetDeliveries.Query());
        [HttpGet("{id}")]
        public async Task<DeliveryDto> GetDelivery(int id)
        {
            return await _mediator.Send(new GetDeliveryById.Query() { Id = id });
        }

        [HttpPost]
        public async Task<ActionResult> CreateDelivery(Delivery delivery) //[FromBody] AddDelivery.Command command)
        {
            int createDeliveryId = await _mediator.Send(new AddDelivery.Command
            {
                Price = delivery.Price,
                ServiceName = delivery.ServiceName,
                TimeSpan = delivery.TimeSpan
            }); //command);
            return CreatedAtAction(nameof(GetDelivery), new { id = createDeliveryId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDelivery(int id, Delivery delivery) //[FromBody] EditDelivery.Command command)
        {
            if (id != delivery.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(new EditDelivery.Command()
            {
                Id = delivery.Id,
                Price = delivery.Price,
                ServiceName = delivery.ServiceName,
                TimeSpan = delivery.TimeSpan
            })); //command));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDelivery(int id)
        {
            await _mediator.Send(new DeleteDelivery.Command { Id = id });
            return NoContent();
        }
    }
}
