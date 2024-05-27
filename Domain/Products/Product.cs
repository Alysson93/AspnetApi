using Flunt.Validations;

namespace AspnetApi.Domain.Products;

public class Product : Entity
{
    public string Description { get; set; } = string.Empty;
    public Category? Category { get; set; }
    public Guid CategoryId { get; set; }
    public bool HasStock { get; set; }

    public Product(string name, string description, Guid categoryId, Category category, string createdBy)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        Category = category;
        Active = true;
        HasStock = true;
        CreatedBy = createdBy;
        EditedBy = createdBy;
        CreatedAt = DateTime.Now;
        EditedAt = DateTime.Now;
        Validate();
    }

    private void Validate() {
        var contract = new Contract<Product>()
            .IsNotNullOrEmpty(Name, "Name", "Nome do produto é um atributo obrigatório.")
            .IsGreaterOrEqualsThan(Name, 2, "Name", "Produto deve ter no mínimo 2 caracteres.")
            .IsNotNullOrEmpty(Description, "Description", "Descrição é um atributo obrigatório")
            .IsNotNull(CategoryId, "CategoryId", "Produto deve ter categoria")
            .IsNotNullOrEmpty(CreatedBy, "CreatedBy", "Nome do criador é um atributo obrigatório.")
            .IsNotNullOrEmpty(EditedBy, "EditedBy", "Nome do editor é um atributo obrigatório.");
        AddNotifications(contract);
    }

}
