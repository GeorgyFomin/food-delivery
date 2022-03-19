using MediatR;
using Microsoft.AspNetCore.Mvc;
using UseCases.API.Discounts;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : Controller
    {
        private readonly IMediator _mediator;
        public DiscountsController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<DiscountDto>> GetDiscounts() => await _mediator.Send(new GetDiscounts.Query());
        [HttpGet("{id}")]
        public async Task<DiscountDto?> GetDiscount(int id) => await _mediator.Send(new GetDiscountById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateDiscount(DiscountDto discountDto)
        {
            if (discountDto == null)
            {
                throw new EntityNotFoundException("DiscountDto not found");
            }
            int createDiscountId = await _mediator.Send(new AddDiscount.Command
            {
                Type = discountDto.Type,
                Size = discountDto.Size
            });
            return CreatedAtAction(nameof(GetDiscount), new { id = createDiscountId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscount(int id, DiscountDto discountDto)
        {
            if (id != discountDto.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(new EditDiscount.Command()
            {
                Id = discountDto.Id,
                Type = discountDto.Type,
                Size = discountDto.Size
            }));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDiscount(int id)
        {
            await _mediator.Send(new DeleteDiscount.Command { Id = id });
            return NoContent();
        }
    }
}
