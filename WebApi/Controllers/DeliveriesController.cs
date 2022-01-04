using Microsoft.AspNetCore.Mvc;
using MediatR;
using UseCases.API.Deliveries;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

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
        public async Task<ActionResult> CreateDelivery(DeliveryDto deliveryDto) //[FromBody] AddDelivery.Command command)
        {
            if (deliveryDto == null)
            {
                throw new EntityNotFoundException("DeliveryDto not found");
            }
            int createDeliveryId = await _mediator.Send(new AddDelivery.Command
            {
                Price = deliveryDto.Price,
                ServiceName = deliveryDto.ServiceName ?? "Noname",
                TimeSpan = deliveryDto.TimeSpan
            }); //command);
            return CreatedAtAction(nameof(GetDelivery), new { id = createDeliveryId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDelivery(int id, DeliveryDto deliveryDto) //[FromBody] EditDelivery.Command command)
        {
            if (id != deliveryDto.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(new EditDelivery.Command()
            {
                Id = deliveryDto.Id,
                Price = deliveryDto.Price,
                ServiceName = deliveryDto.ServiceName ?? "Noname",
                TimeSpan = deliveryDto.TimeSpan
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
