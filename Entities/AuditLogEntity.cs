namespace SampleWebApiAspNetCore.Dtos;

public class AuditLogEntity
{
    public int Id { get; set; }
    public string Username { get; set; } // JWT-dən gələcək
    public string Action { get; set; }   // GET, POST, PUT, DELETE
    public string Endpoint { get; set; } // /api/v1/foods/5
    public string Method { get; set; }   // FoodsController.UpdateFood
    public DateTime Timestamp { get; set; }
    public string RequestBody { get; set; }
}
