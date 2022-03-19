using Microsoft.AspNetCore.Mvc;
using MediatR;
using Entities;
using UseCases.API.Orders;
using UseCases.API.Dto;
using UseCases.API.Exceptions;
using Entities.Domain;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrdersController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<OrderDto>> GetOrders() => await _mediator.Send(new GetOrders.Query());
        [HttpGet("{id}")]
        public async Task<OrderDto?> GetOrder(int id) => await _mediator.Send(new GetOrderById.Query() { Id = id });
        [HttpPost]
        public async Task<ActionResult> CreateOrder(OrderDto orderDto)
        {
            if (orderDto == null)
            {
                throw new EntityNotFoundException("OrderDto not found");
            }
            DeliveryDto? deliveryDto = orderDto.Delivery;
            Delivery? delivery;
            if (deliveryDto == null) delivery = null;
            else delivery = new()
            {
                Price = deliveryDto.Price,
                ServiceName = string.IsNullOrWhiteSpace(deliveryDto.ServiceName) ? "Noname" : deliveryDto.ServiceName,
                TimeSpan = deliveryDto.TimeSpan
            };
            DiscountDto? discountDto = orderDto.Discount;
            Discount? discount;
            if (discountDto == null) discount = null;
            else discount = new() { Size = discountDto.Size, Type = discountDto.Type };
            List<OrderItem> orderItems = new();
            foreach (OrderItemDto orderItemDto in orderDto.OrderElements)
            {
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
                    product = new Product()
                    {
                        ProductsIngredients = productIngredients,
                        Name = string.IsNullOrWhiteSpace(productDto.Name) ? "Noname" : productDto.Name,
                        Price = productDto.Price,
                        Weight = productDto.Weight
                    };
                }
                orderItems.Add(new OrderItem() { Product = product, Quantity = orderItemDto.Quantity });
            }

            int createOrderId = await _mediator.Send(new AddOrder.Command
            {
                Delivery = delivery,
                Discount = discount,
                OrderElements = orderItems
            });
            return CreatedAtAction(nameof(GetOrder), new { id = createOrderId }, null);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            await _mediator.Send(new DeleteOrder.Command { Id = id });
            return NoContent();
        }
    }
}
