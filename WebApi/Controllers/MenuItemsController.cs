using Entities.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UseCases.API.Dto;
using UseCases.API.Exceptions;
using UseCases.API.MenuItems;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : Controller
    {
        private readonly IMediator _mediator;
        public MenuItemsController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<MenuItemDto>> GetMenuItems() => await _mediator.Send(new GetMenuItems.Query());
        [HttpGet("{id}")]
        public async Task<MenuItemDto?> GetMenuItem(int id) => await _mediator.Send(new GetMenuItemById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateMenuItem(MenuItemDto MenuItemDto)
        {
            if (MenuItemDto == null)
            {
                throw new EntityNotFoundException("MenuItemDto not found");
            }
            ProductDto? productDto = MenuItemDto.Product;
            Product? product;
            if (productDto == null) product = null;
            else
            {
                List<ProductIngredient> productIngredients = new();
                foreach (ProductIngredientDto productIngredientDto in productDto.ProductsIngredients)
                {
                    productIngredients.Add(new ProductIngredient()
                    {
                        IngredientId = productIngredientDto.IngredientId,
                        ProductId = productIngredientDto.ProductId
                    });
                }
                product = new()
                {
                    Name = string.IsNullOrWhiteSpace(productDto.Name) ? "Noname" : productDto.Name,
                    Price = productDto.Price,
                    Weight = productDto.Weight,
                    ProductsIngredients = productIngredients
                };
            }
            int createMenuItemId = await _mediator.Send(new AddMenuItem.Command
            {
                Product = product
            });
            return CreatedAtAction(nameof(GetMenuItem), new { id = createMenuItemId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, MenuItemDto menuItemDto)
        {
            if (id != menuItemDto.Id)
            {
                return BadRequest();
            }
            ProductDto? productDto = menuItemDto.Product;
            Product? product = null;
            if (productDto != null)
            {
                List<ProductIngredient> productIngredients = new();
                foreach (ProductIngredientDto productIngredientDto in productDto.ProductsIngredients)
                {
                    productIngredients.Add(new ProductIngredient()
                    {
                        IngredientId = productIngredientDto.IngredientId,
                        ProductId = productIngredientDto.ProductId
                    });
                }
                product = new()
                {
                    Name = string.IsNullOrWhiteSpace(productDto.Name) ? "Noname" : productDto.Name,
                    Price = productDto.Price,
                    Weight = productDto.Weight,
                    ProductsIngredients = productIngredients
                };
            }
            return Ok(await _mediator.Send(new EditMenuItem.Command()
            {
                Id = menuItemDto.Id,
                Product = product
            }));
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteMenuItem(int id)
        {
            await _mediator.Send(new DeleteMenuItem.Command { Id = id });
            return NoContent();
        }
    }
}
