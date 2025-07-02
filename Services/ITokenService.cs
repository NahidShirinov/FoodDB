using SampleWebApiAspNetCore.Entities;

namespace SampleWebApiAspNetCore.Services;

public interface ITokenService
{
    string GenerateToken(UserEntity user);
   
}
