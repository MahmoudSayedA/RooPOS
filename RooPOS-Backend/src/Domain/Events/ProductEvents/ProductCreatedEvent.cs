using Domain.Entities.Products;

namespace Domain.Events.ProductEvents
{
    public class ProductCreatedEvent : BaseEvent
    {
        public Product Product { get; set; }
        public ProductCreatedEvent(Product product) => Product = product;

    }
}
