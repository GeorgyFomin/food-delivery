using MediatR;
using Microsoft.AspNetCore.Mvc;
using UseCases.API.Dto;
using UseCases.API.Exceptions;
using UseCases.API.Ingredients;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public IngredientsController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<IngredientDto>> GetIngredients() => await _mediator.Send(new GetIngredients.Query());
        [HttpGet("{id}")]
        public async Task<IngredientDto> GetIngredient(int id) => await _mediator.Send(new GetIngredientById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateIngredient(IngredientDto ingredientDto) //[FromBody] AddIngredient.Command command)
        {
            if (ingredientDto == null)
            {
                throw new EntityNotFoundException("IngredientDto not found");
            }
            var createIngredientId = await _mediator.Send(new AddIngredient.Command()
            {
                Name = ingredientDto.Name ?? "Noname"
                ,
                ProductId = ingredientDto.ProductId
            }); // command);
            return CreatedAtAction(nameof(GetIngredient), new { id = createIngredientId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredient(int id, IngredientDto ingredientDto) //[FromBody] EditIngredient.Command command)
        {
            if (id != ingredientDto.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(new EditIngredient.Command()
            {
                Id = ingredientDto.Id,
                Name = ingredientDto.Name ?? "Noname"
            })); //command));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            await _mediator.Send(new DeleteIngredient.Command { Id = id });
            return NoContent();
        }
    }
}
