namespace EasyRoster.Api.Models;
public sealed class Customer {
 public int Id { get; set; }
 public string Type { get; set; } = "Person";
 public string FirstName { get; set; } = string.Empty;
 public string Surname { get; set; } = string.Empty;
 public string Email { get; set; } = string.Empty;
 public string Cellphone { get; set; } = string.Empty;
 public decimal AmountTotal { get; set; }
 public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
 public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
