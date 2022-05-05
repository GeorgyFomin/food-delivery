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
        public async Task<ActionResult> CreateMenuItem(MenuItemDto menuItemDto)
        {
            if (menuItemDto == null)
            {
                throw new EntityNotFoundException("MenuItemDto not found");
            }
            int createMenuItemId = await _mediator.Send(new AddMenuItem.Command
            {
                Product = menuItemDto.Product
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
            return Ok(await _mediator.Send(new EditMenuItem.Command()
            {
                Id = menuItemDto.Id,
                Product = menuItemDto.Product
            }));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMenuItem(int id)
        {
            await _mediator.Send(new DeleteMenuItem.Command { Id = id });
            return NoContent();
        }
    }
}
