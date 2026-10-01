namespace EasyRoster.Api.Contracts;
public sealed record CustomerResponse(int Id,string Type,string FirstName,string Surname,string Email,string Cellphone,decimal AmountTotal);
