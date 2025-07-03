using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Helpers;
using SampleWebApiAspNetCore.Models;
using System.Linq.Dynamic.Core;
using AutoMapper;
using SampleWebApiAspNetCore.Services;
using Microsoft.EntityFrameworkCore;

namespace SampleWebApiAspNetCore.Repositories
{
    public class FoodSqlRepository : IFoodRepository
    {
        private readonly FoodDbContext _foodDbContext;
        private readonly IAuditLogService _auditLogService;

        public FoodSqlRepository(FoodDbContext foodDbContext, IAuditLogService auditLogService)
        {
            _foodDbContext = foodDbContext;
            _auditLogService = auditLogService;
        }

        public FoodEntity GetSingle(int id)
        {
            return _foodDbContext.FoodItems
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted); // Soft delete yoxlama
        }

        public void Add(FoodEntity item)
        {
            if (_foodDbContext.FoodItems.Any(f => f.Id == item.Id))
            {
                throw new InvalidOperationException($"Food item with Id {item.Id} already exists.");
            }

            _foodDbContext.FoodItems.Add(item);
        }

        public void Delete(int id)
        {
            var foodItem = GetSingle(id);
            if (foodItem == null)
            {
                throw new InvalidOperationException($"Food item with Id {id} does not exist or is already deleted.");
            }

            foodItem.IsDeleted = true;  // Soft delete
        }

        public FoodEntity Update(int id, FoodEntity item)
        {
            var existingFood = _foodDbContext.FoodItems.FirstOrDefault(f => f.Id == id && !f.IsDeleted);
            if (existingFood == null)
            {
                throw new InvalidOperationException($"Food item with Id {id} does not exist or is deleted.");
            }

            existingFood.Name = item.Name;
            existingFood.Type = item.Type;
            existingFood.Calories = item.Calories;

            _foodDbContext.FoodItems.Update(existingFood);
            return existingFood;
        }

        public IQueryable<FoodEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<FoodEntity> _allItems = _foodDbContext.FoodItems
                .Where(f => !f.IsDeleted)
                .OrderBy(queryParameters.OrderBy, queryParameters.IsDescending());

            if (queryParameters.HasQuery())
            {
                var loweredQuery = queryParameters.Query.ToLowerInvariant();

                _allItems = _allItems.Where(x =>
                    x.Calories.ToString().Contains(loweredQuery) ||
                    (x.Name != null && x.Name.ToLowerInvariant().Contains(loweredQuery))
                );
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _foodDbContext.FoodItems.Count(f => !f.IsDeleted);
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }

        public ICollection<FoodEntity> GetRandomMeal()
        {
            List<FoodEntity> toReturn = new List<FoodEntity>
            {
                GetRandomItem("Starter"),
                GetRandomItem("Main"),
                GetRandomItem("Dessert")
            };

            return toReturn;
        }

        private FoodEntity GetRandomItem(string type)
        {
            return _foodDbContext.FoodItems
                .Where(x => x.Type == type && !x.IsDeleted)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}
