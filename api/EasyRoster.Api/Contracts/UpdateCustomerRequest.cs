using System.ComponentModel.DataAnnotations;
namespace EasyRoster.Api.Contracts;
public sealed record UpdateCustomerRequest(
 [Required, MaxLength(20)] string Type,
 [Required, MaxLength(100)] string FirstName,
 [Required, MaxLength(100)] string Surname,
 [Required, EmailAddress, MaxLength(255)] string Email,
 [Required, MaxLength(30)] string Cellphone,
 [Range(0, double.MaxValue)] decimal AmountTotal);
