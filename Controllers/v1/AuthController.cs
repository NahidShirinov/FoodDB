using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Repositories;
using SampleWebApiAspNetCore.Services;

namespace SampleWebApiAspNetCore.Controllers.v1;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly string _jwtKey;
    private readonly IMapper _mapper;


    public AuthController(
        IUserService userService,
        ITokenService tokenService,
        IConfiguration configuration,
        IMapper mapper
       )
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
            return Unauthorized("Invalid credentials");
        }

        var key = Encoding.ASCII.GetBytes(_jwtKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var auth = tokenHandler.WriteToken(token);

        return Ok(new { auth });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserCreateDto dto)
    {
        if (_userService.Exists(dto.Username))
            return BadRequest(new { error = "User already exists" });

        var user = await _userService.CreateAsync(dto.Username, dto.Password);
        var token = _tokenService.GenerateToken(user);

        return Ok(new
        {
            user = _mapper.Map<UserResponseDto>(user),
            token
        });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users.Select(u => _mapper.Map<UserResponseDto>(u)));
    }

    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var user = await _userService.GetByUsername(username);
        if (user == null) return NotFound();

        return Ok(_mapper.Map<UserResponseDto>(user));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUserAsync(Guid id)
    {
        var user = await _userService.Finduser(id);
        if (user == null || user.IsDeleted)
            return NotFound();

        user.IsDeleted = true;
         _userService.Save();
        return StatusCode(200,"deleted user succesfully");
    }


}