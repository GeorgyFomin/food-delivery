using Microsoft.AspNetCore.Mvc;
using MediatR;
using UseCases.API.Products;
using UseCases.API.Dto;
using UseCases.API.Exceptions;
using Entities.Domain;
using Persistence.MsSql;

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
        public async Task<ActionResult> CreateProduct(ProductDto productDto) //[FromBody] AddProduct.Command command)
        {
            if (productDto == null)
            {
                throw new EntityNotFoundException("ProductDto not found");
            }
            var createProductId = await _mediator.Send(new AddProduct.Command()
            {
                Name = productDto.Name ?? "Noname",
                Ingredients = productDto.Ingredients,
                Price = productDto.Price,
                Weight = productDto.Weight
            }); //command);
            return CreatedAtAction(nameof(GetProduct), new { id = createProductId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDto productDto) //[FromBody] EditProduct.Command command)
        {
            if (id != productDto.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(
                new EditProduct.Command
                {
                    Id = productDto.Id,
                    Weight = productDto.Weight,
                    Ingredients = productDto.Ingredients,
                    Name = productDto.Name ?? "Noname",
                    Price = productDto.Price
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
