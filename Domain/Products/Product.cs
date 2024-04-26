namespace AspnetApi.Domain.Products;

public class Product : Entity
{
    public string Description { get; set; } = string.Empty;
    public Category Category { get; set; } = new();
    public Guid CategoryId { get; set; }
    public bool HasStock { get; set; }
}
