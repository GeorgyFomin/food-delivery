using Entities.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UseCases.API.Dto;
using UseCases.API.Exceptions;
using UseCases.API.Menus;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController : Controller
    {
        private readonly IMediator _mediator;
        public MenusController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<MenuDto>> GetMenus() => await _mediator.Send(new GetMenus.Query());
        [HttpGet("{id}")]
        public async Task<MenuDto?> GetMenu(int id) => await _mediator.Send(new GetMenuById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateMenu(MenuDto menuDto)
        {
            if (menuDto == null)
            {
                throw new EntityNotFoundException("MenuDto not found");
            }
            List<MenuItem> menuItems = new();
            foreach (MenuItemDto menuItemDto in menuDto.MenuItems)
            {
                ProductDto? productDto = menuItemDto.Product;
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
                    product = new Product()
                    {
                        ProductsIngredients = productIngredients,
                        Name = string.IsNullOrWhiteSpace(productDto.Name) ? "Noname" : productDto.Name,
                        Price = productDto.Price,
                        Weight = productDto.Weight
                    };
                }
                menuItems.Add(new MenuItem() { Product = product });
            }

            int createMenuId = await _mediator.Send(new AddMenu.Command
            {
                MenuItems = menuItems
            });
            return CreatedAtAction(nameof(GetMenu), new { id = createMenuId }, null);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, MenuDto menuDto)
        {
            if (id != menuDto.Id)
            {
                return BadRequest();
            }
            List<MenuItem> menuItems = new();
            foreach (MenuItemDto menuItemDto in menuDto.MenuItems)
            {
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
                menuItems.Add(new MenuItem { Product = product });
            }
            return Ok(await _mediator.Send(new EditMenu.Command()
            {
                Id = menuDto.Id,
                MenuItems = menuItems
            }));
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteMenu(int id)
        {
            await _mediator.Send(new DeleteMenu.Command { Id = id });
            return NoContent();
        }
    }
}
