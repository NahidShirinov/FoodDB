using Microsoft.EntityFrameworkCore;
using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Services;

namespace SampleWebApiAspNetCore.Repositories;

public class UserService : IUserService
{
    private readonly FoodDbContext _context;

    public UserService(FoodDbContext context)
    {
        _context = context;
    }

    public UserEntity Authenticate(string username, string password)
    {
        return _context.Users
            .FirstOrDefault(u => u.Username == username && u.Password == password);
    }

    public bool Exists(string username)
    {
        return _context.Users.Any(u => u.Username == username);
    }

    public async Task<UserEntity> CreateAsync(string dtoUsername, string dtoPassword)
    {
        if (Exists(dtoUsername))
        {
            throw new InvalidOperationException("Bu istifadəçi artıq mövcuddur.");
        }

        var user = new UserEntity
        {
        
            Username = dtoUsername,
            Password = dtoPassword
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

   public async Task<bool> DeleteUserAsync(Guid id)
{
    var user = await _context.Users.FindAsync(id);
    if (user == null || user.IsDeleted)
        return false;

    user.IsDeleted = true;
    await _context.SaveChangesAsync();
    return true;
}


    public async Task<UserEntity> GetByUsername(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<List<UserEntity>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<UserEntity?> Finduser(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public bool Save()
    {
        return (_context.SaveChanges() >= 0);
    }
}