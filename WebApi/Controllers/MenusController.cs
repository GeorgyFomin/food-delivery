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

            int createMenuId = await _mediator.Send(new AddMenu.Command
            {
                MenuItems = menuDto.MenuItems
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
            return Ok(await _mediator.Send(new EditMenu.Command()
            {
                Id = menuDto.Id,
                MenuItems = menuDto.MenuItems
            }));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMenu(int id)
        {
            await _mediator.Send(new DeleteMenu.Command { Id = id });
            return NoContent();
        }
    }
}
