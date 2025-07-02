using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Repositories;

namespace SampleWebApiAspNetCore.Services
{
    public class SeedDataService : ISeedDataService
    {
        public void Initialize(FoodDbContext context)
        {
            context.FoodItems.Add(new FoodEntity() { Calories = 1000, Type = "Starter", Name = "Lasagne" });
            context.FoodItems.Add(new FoodEntity() { Calories = 1100, Type = "Main", Name = "Hamburger" });
            context.FoodItems.Add(new FoodEntity() { Calories = 1200, Type = "Dessert", Name = "Spaghetti" });
            context.FoodItems.Add(new FoodEntity() { Calories = 1300, Type = "Starter", Name = "Pizza" });

            context.SaveChanges();
        }
    }
}
