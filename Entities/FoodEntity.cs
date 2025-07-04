namespace SampleWebApiAspNetCore.Entities
{
    public class FoodEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int Calories { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int? CategoryId { get; set; }   // Foreign key
        public CategoryEntity Category { get; set; }  // Navigation property
        //public DateTime Created { get; set; }
    }
}
