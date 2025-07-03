namespace SampleWebApiAspNetCore.Entities
{
    public class FoodEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int Calories { get; set; }
        public bool IsDeleted { get; set; } = false;
        //public DateTime Created { get; set; }
    }
}
