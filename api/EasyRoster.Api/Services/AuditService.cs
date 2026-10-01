using System.Text.Json;
using EasyRoster.Api.Data;
using EasyRoster.Api.Models;
namespace EasyRoster.Api.Services;
public sealed class AuditService(AppDbContext db) {
 public async Task WriteAsync(string entity,string id,string action,object? changes,CancellationToken ct) {
  db.AuditLogs.Add(new AuditLog { EntityName=entity, EntityId=id, Action=action, Changes=changes is null?null:JsonSerializer.Serialize(changes) });
  await db.SaveChangesAsync(ct);
 }
}
