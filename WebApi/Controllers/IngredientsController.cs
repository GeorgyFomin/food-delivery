using Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IEnumerable<Ingredient>> GetIngredients() => await _mediator.Send(new GetIngredients.Query());
        [HttpGet("{id}")]
        public async Task<Ingredient> GetIngredient(int id) => await _mediator.Send(new GetIngredientById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateIngredient(Ingredient ingredient)// [FromBody] AddIngredient.Command command)
        {
            var createIngredientId = await _mediator.Send(new AddIngredient.Command { Name = ingredient.Name });// command);
            return CreatedAtAction(nameof(GetIngredient), new { id = createIngredientId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredient(int id, Ingredient ingredient)
        {
            if (id != ingredient.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(
                new EditIngredient.Command { Id = ingredient.Id, Name = ingredient.Name }));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            await _mediator.Send(new DeleteIngredient.Command { Id = id });
            return NoContent();
        }
    }
}
