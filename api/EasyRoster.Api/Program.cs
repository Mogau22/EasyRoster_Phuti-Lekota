using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using EasyRoster.Api.Data;
using EasyRoster.Api.Middleware;
using EasyRoster.Api.Observability;
using EasyRoster.Api.Services;
using Microsoft.EntityFrameworkCore;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(o=>o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<AuditService>(); builder.Services.AddApplicationTelemetry();
builder.Services.AddCors(o=>o.AddPolicy("Frontend",p=>p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddRateLimiter(o=>o.AddFixedWindowLimiter("api",x=>{x.PermitLimit=100;x.Window=TimeSpan.FromMinutes(1);x.QueueLimit=0;x.QueueProcessingOrder=QueueProcessingOrder.OldestFirst;}));
var app=builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>(); app.UseHttpsRedirection(); app.UseCors("Frontend"); app.UseRateLimiter();
if(app.Environment.IsDevelopment()){app.UseSwagger();app.UseSwaggerUI();}
app.MapControllers().RequireRateLimiting("api"); app.Run();
