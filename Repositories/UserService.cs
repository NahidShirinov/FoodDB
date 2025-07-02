using Microsoft.AspNetCore.Mvc;
using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Services;

namespace SampleWebApiAspNetCore.Repositories;

public class UserService : IUserService
{
    private List<UserEntity> _user = new()
    {
        new UserEntity() { Username = "admin", Password = "password" },
        new UserEntity() { Username = "nahid", Password = "1234" }
    };

    public UserEntity Authenticate(string username, string password)
    {
        return _user.FirstOrDefault(x => x.Username == username && x.Password == password);
    }

    public bool Exists(string username)
    {
        return _user.Any(x => x.Username == username);
    }

    public Task<UserEntity> CreateAsync(string dtoUsername, string dtoPassword)
    {
        if (_user.Any(x => x.Username == dtoUsername && x.Password == dtoPassword))
        {
            throw new InvalidOperationException("Bu istifadəçi artıq mövcuddur.");
        }

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Username = dtoUsername,
            Password = dtoPassword
        };


        _user.Add(user);

        return Task.FromResult(user);
    }

    public Task<bool> DeleteUserAsync(Guid id)
    {
        var user = _user.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return Task.FromResult(false);

        _user.Remove(user);
        return Task.FromResult(true);
    }

    public Task<UserEntity> GetByUsername(string username)
    {
        var user = _user.FirstOrDefault(u => u.Username == username);
        return GetByUsername(username);
    }


}
