using Newtonsoft.Json;
using SampleWebApiAspNetCore.Dtos;
using Newtonsoft.Json;
using SampleWebApiAspNetCore.Services;

namespace SampleWebApiAspNetCore.Repositories;

public class AuditLogService : IAuditLogService
{
    private readonly FoodDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogService( FoodDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(string action, string endpoint, string method, object? requestBody)
    {
        var username = _httpContextAccessor.HttpContext.User.Identity.Name;
        var audit = new AuditLogEntity
        {
            Username = username,
            Action = action,
            Endpoint = endpoint,
            Method = method,
            RequestBody = requestBody != null
                ? JsonConvert.SerializeObject(requestBody)
                : string.Empty,
            Timestamp = DateTime.UtcNow
        };
        _context.AuditLogs.Add(audit);
        await _context.SaveChangesAsync();
    }
}