using SampleWebApiAspNetCore.Repositories;
using Microsoft.AspNetCore.Http;
using SampleWebApiAspNetCore.Entities;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SampleWebApiAspNetCore.Middlware;

public class AuditLogMiddleware
{
    public readonly RequestDelegate _next;

    public AuditLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, FoodDbContext dbContext)
    {
       
       //dbContext.Request.EnableBuffering();
        
    }
}