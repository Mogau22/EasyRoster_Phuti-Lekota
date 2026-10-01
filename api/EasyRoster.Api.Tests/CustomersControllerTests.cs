using EasyRoster.Api.Contracts;
using EasyRoster.Api.Controllers;
using EasyRoster.Api.Data;
using EasyRoster.Api.Models;
using EasyRoster.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace EasyRoster.Api.Tests;

public sealed class CustomersControllerTests
{
    [Fact]
    public async Task Get_WithSearch_ReturnsMatchingCustomersOnly()
    {
        await using var db = CreateDb();
        db.Customers.AddRange(
            Customer("Simple", "Joe", "simple.joe@example.com"),
            Customer("Jane", "Smith", "jane@example.com"));
        await db.SaveChangesAsync();
        var sut = CreateController(db);

        var result = await sut.Get("simple", CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var customers = Assert.IsAssignableFrom<IEnumerable<CustomerResponse>>(ok.Value).ToList();
        var customer = Assert.Single(customers);
        Assert.Equal("Simple", customer.FirstName);
    }

    [Fact]
    public async Task GetById_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        await using var db = CreateDb();
        var sut = CreateController(db);

        var result = await sut.GetById(999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_PersistsCustomer_AndWritesAuditEntry()
    {
        await using var db = CreateDb();
        var sut = CreateController(db);
        var request = Request("Simple", "Joe", "simple.joe@example.com");

        var result = await sut.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<CustomerResponse>(created.Value);
        Assert.True(response.Id > 0);
        Assert.Equal("simple.joe@example.com", response.Email);
        Assert.Single(await db.Customers.ToListAsync());
        var audit = Assert.Single(await db.AuditLogs.ToListAsync());
        Assert.Equal("CustomerCreated", audit.Action);
        Assert.Equal(response.Id.ToString(), audit.EntityId);
    }

    [Fact]
    public async Task Update_WhenCustomerExists_UpdatesCustomer_AndWritesAuditEntry()
    {
        await using var db = CreateDb();
        var customer = Customer("Simple", "Joe", "old@example.com");
        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        var sut = CreateController(db);
        var request = new UpdateCustomerRequest("Person", "Simple", "Joe", "new@example.com", "0821234567", 250m);

        var result = await sut.Update(customer.Id, request, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        var saved = await db.Customers.FindAsync(customer.Id);
        Assert.NotNull(saved);
        Assert.Equal("new@example.com", saved.Email);
        Assert.Equal(250m, saved.AmountTotal);
        Assert.Contains(await db.AuditLogs.ToListAsync(), x => x.Action == "CustomerUpdated" && x.EntityId == customer.Id.ToString());
    }

    [Fact]
    public async Task Update_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        await using var db = CreateDb();
        var sut = CreateController(db);

        var result = await sut.Update(999, new UpdateCustomerRequest("Person", "A", "B", "a@b.com", "0821234567", 0m), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
        Assert.Empty(await db.AuditLogs.ToListAsync());
    }

    [Fact]
    public async Task Delete_WhenCustomerExists_RemovesCustomer_AndWritesAuditEntry()
    {
        await using var db = CreateDb();
        var customer = Customer("Simple", "Joe", "simple.joe@example.com");
        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        var sut = CreateController(db);

        var result = await sut.Delete(customer.Id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(await db.Customers.ToListAsync());
        Assert.Contains(await db.AuditLogs.ToListAsync(), x => x.Action == "CustomerDeleted" && x.EntityId == customer.Id.ToString());
    }

    [Fact]
    public async Task Delete_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        await using var db = CreateDb();
        var sut = CreateController(db);

        var result = await sut.Delete(999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    private static CustomersController CreateController(AppDbContext db) =>
        new(db, new AuditService(db), NullLogger<CustomersController>.Instance);

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static Customer Customer(string firstName, string surname, string email) => new()
    {
        Type = "Person", FirstName = firstName, Surname = surname, Email = email,
        Cellphone = "0821234567", AmountTotal = 0m
    };

    private static CreateCustomerRequest Request(string firstName, string surname, string email) =>
        new("Person", firstName, surname, email, "0821234567", 0m);
}
