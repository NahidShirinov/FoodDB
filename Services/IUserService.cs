using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Repositories;
namespace SampleWebApiAspNetCore.Services
{
    public interface IUserService
    {
        UserEntity Authenticate(string username, string password);
        bool Exists(string username);
        Task<UserEntity> CreateAsync(string username, string password);
        Task<bool> DeleteUserAsync(Guid id);


        Task<UserEntity> GetByUsername(string username);
        Task<List<UserEntity>> GetAllAsync();
        Task<UserEntity?> Finduser(Guid id);
        bool Save();

        
    }
}
