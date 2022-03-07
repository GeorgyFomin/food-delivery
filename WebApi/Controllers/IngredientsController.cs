using Entities.Domain;
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
        public async Task<IEnumerable<IngredientDto?>> GetIngredients() => await _mediator.Send(new GetIngredients.Query());
        [HttpGet("{id}")]
        public async Task<IngredientDto?> GetIngredient(int id) => await _mediator.Send(new GetIngredientById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult?> CreateIngredient(IngredientDto ingredientDto) //[FromBody] AddIngredient.Command command)
        {
            if (ingredientDto == null)
            {
                throw new EntityNotFoundException("IngredientDto not found");
            }
            List<ProductIngredient> productIngredients = new();
            if (ingredientDto.ProductsIngredients != null)
            {
                foreach (ProductIngredientDto productIngredientDto in ingredientDto.ProductsIngredients)
                {
                    productIngredients.Add(new ProductIngredient() { IngredientId = productIngredientDto.IngredientId, ProductId = productIngredientDto.ProductId });
                }
            }
            var createIngredientId = await _mediator.Send(new AddIngredient.Command()
            {
                Name = string.IsNullOrWhiteSpace(ingredientDto.Name) ? "Noname" : ingredientDto.Name,
                ProductsIngredients = productIngredients
            });
            return CreatedAtAction(nameof(GetIngredient), new { id = createIngredientId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult?> UpdateIngredient(int id, IngredientDto ingredientDto) //[FromBody] EditIngredient.Command command)
        {
            if (id != ingredientDto.Id)
            {
                return BadRequest();
            }
            List<ProductIngredient> productIngredients = new();
            if (ingredientDto.ProductsIngredients != null)
            {
                foreach (ProductIngredientDto productIngredientDto in ingredientDto.ProductsIngredients)
                {
                    productIngredients.Add(new ProductIngredient() { IngredientId = productIngredientDto.IngredientId, ProductId = productIngredientDto.ProductId });
                }
            }
            return Ok(await _mediator.Send(new EditIngredient.Command()
            {
                Id = ingredientDto.Id,
                Name = string.IsNullOrWhiteSpace(ingredientDto.Name) ? "Noname" : ingredientDto.Name,
                ProductsIngredients = productIngredients
            }));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult?> DeleteIngredient(int id)
        {
            await _mediator.Send(new DeleteIngredient.Command { Id = id });
            return NoContent();
        }
    }
}
