using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Models;
using System.Linq;

namespace SampleWebApiAspNetCore.Repositories
{
    public interface IFoodRepository
    {
        FoodEntity GetSingle(int id);
        void Add(FoodEntity item);
        void Delete(int id);  // Soft delete kimi işləyəcək
        FoodEntity Update(int id, FoodEntity item);
        IQueryable<FoodEntity> GetAll(QueryParameters queryParameters);
        ICollection<FoodEntity> GetRandomMeal();
        int Count();
        bool Save();
        bool CategoryExists(int categoryId);
    }
}