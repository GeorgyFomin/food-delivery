using Microsoft.AspNetCore.Mvc;
using MediatR;
using UseCases.API.Products;
using Entities.Domain;
using UseCases.API.Deliveries.Dto;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<ProductDto>> GetProducts() => await _mediator.Send(new GetProducts.Query());
        [HttpGet("{id}")]
        public async Task<ProductDto> GetProduct(int id) => await _mediator.Send(new GetProductById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateProduct(Product product) //[FromBody] AddProduct.Command command)
        {
            var createProductId = await _mediator.Send(new AddProduct.Command()
            {
                Name = product.Name,
                Ingredients = product.Ingredients,
                Price = product.Price,
                Weight = product.Weight
            }); //command);
            return CreatedAtAction(nameof(GetProduct), new { id = createProductId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product) //[FromBody] EditProduct.Command command)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(
                new EditProduct.Command
                {
                    Id = product.Id,
                    Weight = product.Weight,
                    Ingredients = product.Ingredients,
                    Name = product.Name,
                    Price = product.Price
                }));
            //command));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            await _mediator.Send(new DeleteProduct.Command { Id = id });
            return NoContent();
        }
    }
}
