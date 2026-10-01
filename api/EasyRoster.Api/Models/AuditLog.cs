namespace EasyRoster.Api.Models;
public sealed class AuditLog {
 public long Id { get; set; }
 public string EntityName { get; set; } = string.Empty;
 public string EntityId { get; set; } = string.Empty;
 public string Action { get; set; } = string.Empty;
 public string? ChangedBy { get; set; }
 public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
 public string? Changes { get; set; }
}
