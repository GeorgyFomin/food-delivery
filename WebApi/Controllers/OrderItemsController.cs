﻿using Microsoft.AspNetCore.Mvc;
using MediatR;
using Entities;
using UseCases.API.OrderItems;
using UseCases.API.Dto;
using UseCases.API.Exceptions;
using Entities.Domain;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderItemsController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<OrderItemDto>> GetOrderItems() => await _mediator.Send(new GetOrderItems.Query());
        [HttpGet("{id}")]
        public async Task<OrderItemDto?> GetOrderItem(int id) => await _mediator.Send(new GetOrderItemById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateOrderItem(OrderItemDto orderItemDto)
        {
            if (orderItemDto == null)
            {
                throw new EntityNotFoundException("OrderItemDto not found");
            }
            ProductDto? productDto = orderItemDto.Product;
            Product? product;
            if (productDto == null) product = null;
            else
            {
                List<ProductIngredient> productIngredients = new();
                if (productDto.ProductsIngredients != null)
                {
                    foreach (ProductIngredientDto productIngredientDto in productDto.ProductsIngredients)
                    {
                        productIngredients.Add(new ProductIngredient() { IngredientId = productIngredientDto.IngredientId, ProductId = productIngredientDto.ProductId });
                    }
                }
                product = new() { Name = productDto.Name, Price = productDto.Price, Weight = productDto.Weight, ProductsIngredients = productIngredients };
            }
            int createOrderItemId = await _mediator.Send(new AddOrderItem.Command
            {
                Product = product,
                Quantity = orderItemDto.Quantity
            });
            return CreatedAtAction(nameof(GetOrderItem), new { id = createOrderItemId }, null);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteOrderItem(int id)
        {
            await _mediator.Send(new DeleteOrderItem.Command { Id = id });
            return NoContent();
        }
    }
}
