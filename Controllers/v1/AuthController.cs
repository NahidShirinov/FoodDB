using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Services;

namespace SampleWebApiAspNetCore.Controllers.v1;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase

{
    private readonly IUserService _userService;
    private readonly string _jwtKey;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthController(
        IUserService userService,
        ITokenService tokenService,
        IConfiguration configuration,
        IMapper mapper)
    {
        _userService = userService;
        _tokenService = tokenService;
        _jwtKey = configuration["Jwt:Key"];
        _mapper = mapper;
    }
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        var user = _userService.Authenticate(loginDto.Username, loginDto.Password);
        if (user == null)
        {
            return Unauthorized("invalid credentials");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtKey);
        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
            )
        };
        var token = tokenHandler.CreateToken(tokenDescription);
        var auth = tokenHandler.WriteToken(token);
        return Ok(new {auth});

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserCreateDto dto)
    {
        if (_userService.Exists(dto.Username))
            return BadRequest(new { error = "User already exists" });

        // Burada UserEntity tipində olmalı
        UserEntity user = await _userService.CreateAsync(dto.Username, dto.Password);

        var token = _tokenService.GenerateToken(user);

        return Ok(new
        {
            user = _mapper.Map<UserResponseDto>(user),
            token = token
        });
    }
    [HttpGet("username/{username}")]
    public async Task<IActionResult>  GetUserByUsername(string username)
    {
        var user = _userService.GetByUsername(username);
        if (user == null) return NotFound();

        return Ok(_mapper.Map<UserResponseDto>(user));
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

}