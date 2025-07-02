using Microsoft.EntityFrameworkCore;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;

namespace SampleWebApiAspNetCore.Repositories
{
    public class FoodDbContext : DbContext
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options)
            : base(options)
        {
        }

        public DbSet<FoodEntity> FoodItems { get; set; } = null!;
        public DbSet<AuditLogEntity>AuditLogs { get; set; } = null!;
        public DbSet<UserEntity> Users { get; set; }
    }
    
}
