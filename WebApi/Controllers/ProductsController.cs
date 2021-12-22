using Microsoft.AspNetCore.Mvc;
using MediatR;
using Entities;
using UseCases.API.Products;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<Product>> GetProducts() => await _mediator.Send(new GetProducts.Query());
        [HttpGet("{id}")]
        public async Task<Product> GetProduct(int id) => await _mediator.Send(new GetProductById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] AddProduct.Command command)
        {
            var createProductId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProduct), new { id = createProductId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(
                new EditProduct.Command { Id = product.Id, Name = product.Name, Weight = product.Weight, Price = product.Price, Ingredients = product.Ingredients }));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            await _mediator.Send(new DeleteProduct.Command { Id = id });
            return NoContent();
        }
    }
}
