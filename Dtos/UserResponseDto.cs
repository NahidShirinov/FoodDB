namespace SampleWebApiAspNetCore.Dtos;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public bool IsDeleted { get; set; } = false; 
}