namespace SampleWebApiAspNetCore.Entities;

public class CategoryEntity
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Bir category-yə aid food-lar (optional, navigation property)
    public ICollection<FoodEntity> Foods { get; set; }
}