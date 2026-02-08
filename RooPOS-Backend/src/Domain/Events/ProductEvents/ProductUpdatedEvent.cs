using Domain.Entities.Products;

namespace Domain.Events.ProductEvents
{
    public class ProductUpdatedEvent : BaseEvent
    {
        public Product Product { get; set; }
        public ProductUpdatedEvent(Product product) => Product = product;

    }
}
