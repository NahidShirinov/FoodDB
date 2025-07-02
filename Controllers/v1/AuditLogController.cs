using Microsoft.AspNetCore.Mvc;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Repositories;

namespace SampleWebApiAspNetCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogController: ControllerBase
{
    private readonly FoodDbContext _context;

    public AuditLogController(FoodDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<AuditLogEntity>> GetAuditLogs()
    {
        return Ok(_context.AuditLogs.OrderByDescending(x=>x.Timestamp).ToList());
    }
}