namespace SampleWebApiAspNetCore.Services;

public interface  IAuditLogService

{
    /// <summary>
    /// Audit log yazır (user, action, endpoint, method, request body və s.)
    /// </summary>
    /// <param name="action">HTTP action: GET, POST və s.</param>
    /// <param name="endpoint">API endpoint: /api/foods/5</param>
    /// <param name="method">Controller metodu: GetFood və s.</param>
    /// <param name="requestBody">İstəyə göndərilən obyekt (body)</param>
    Task LogAsync(string action, string endpoint, string method, object? requestBody);
}