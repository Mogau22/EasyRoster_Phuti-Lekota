using System.ComponentModel.DataAnnotations;
using EasyRoster.Api.Contracts;

namespace EasyRoster.Api.Tests;

public sealed class ValidationTests
{
    [Fact]
    public void CreateRequest_WithValidValues_IsValid()
    {
        var request = new CreateCustomerRequest("Person", "Simple", "Joe", "simple.joe@example.com", "0821234567", 0m);
        Assert.Empty(Validate(request));
    }

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }
}
