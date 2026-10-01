using EasyRoster.Api.Contracts;
using EasyRoster.Api.Data;
using EasyRoster.Api.Models;
using EasyRoster.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EasyRoster.Api.Controllers;
[ApiController, Route("api/[controller]")]
public sealed class CustomersController(AppDbContext db, AuditService audit, ILogger<CustomersController> logger) : ControllerBase {
 [HttpGet]
 public async Task<ActionResult<IEnumerable<CustomerResponse>>> Get([FromQuery]string? search,CancellationToken ct) {
  var q=db.Customers.AsNoTracking();
  if(!string.IsNullOrWhiteSpace(search)){ var s=search.Trim(); q=q.Where(x=>x.FirstName.Contains(s)||x.Surname.Contains(s)||x.Email.Contains(s)||x.Cellphone.Contains(s)); }
  return Ok(await q.OrderBy(x=>x.Surname).ThenBy(x=>x.FirstName).Select(x=>new CustomerResponse(x.Id,x.Type,x.FirstName,x.Surname,x.Email,x.Cellphone,x.AmountTotal)).ToListAsync(ct));
 }
 [HttpGet("{id:int}")]
 public async Task<ActionResult<CustomerResponse>> GetById(int id,CancellationToken ct) {
  var x=await db.Customers.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id,ct);
  return x is null?NotFound():Ok(new CustomerResponse(x.Id,x.Type,x.FirstName,x.Surname,x.Email,x.Cellphone,x.AmountTotal));
 }
 [HttpPost]
 public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest r,CancellationToken ct) {
  var x=new Customer{Type=r.Type,FirstName=r.FirstName,Surname=r.Surname,Email=r.Email,Cellphone=r.Cellphone,AmountTotal=r.AmountTotal};
  db.Customers.Add(x); await db.SaveChangesAsync(ct); await audit.WriteAsync("Customer",x.Id.ToString(),"CustomerCreated",new{x.Email},ct);
  logger.LogInformation("Customer {CustomerId} created",x.Id);
  var response=new CustomerResponse(x.Id,x.Type,x.FirstName,x.Surname,x.Email,x.Cellphone,x.AmountTotal);
  return CreatedAtAction(nameof(GetById),new{id=x.Id},response);
 }
 [HttpPut("{id:int}")]
 public async Task<IActionResult> Update(int id,UpdateCustomerRequest r,CancellationToken ct) {
  var x=await db.Customers.FindAsync([id],ct); if(x is null)return NotFound();
  var before=new{x.Type,x.FirstName,x.Surname,x.Email,x.Cellphone,x.AmountTotal};
  x.Type=r.Type;x.FirstName=r.FirstName;x.Surname=r.Surname;x.Email=r.Email;x.Cellphone=r.Cellphone;x.AmountTotal=r.AmountTotal;x.UpdatedAtUtc=DateTime.UtcNow;
  await db.SaveChangesAsync(ct); await audit.WriteAsync("Customer",id.ToString(),"CustomerUpdated",new{Before=before,After=r},ct); logger.LogInformation("Customer {CustomerId} updated",id); return NoContent();
 }
 [HttpDelete("{id:int}")]
 public async Task<IActionResult> Delete(int id,CancellationToken ct) {
  var x=await db.Customers.FindAsync([id],ct); if(x is null)return NotFound(); db.Customers.Remove(x); await db.SaveChangesAsync(ct); await audit.WriteAsync("Customer",id.ToString(),"CustomerDeleted",null,ct); logger.LogInformation("Customer {CustomerId} deleted",id); return NoContent();
 }
}
