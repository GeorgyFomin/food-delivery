﻿using Entities;
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
        public async Task<ActionResult> CreateIngredient([FromBody] AddIngredient.Command command)
        {
            var createIngredientId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetIngredient), new { id = createIngredientId }, null);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            await _mediator.Send(new DeleteIngredient.Command { Id = id });
            return NoContent();
        }
    }
}