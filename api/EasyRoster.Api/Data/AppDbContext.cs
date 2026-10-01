using EasyRoster.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace EasyRoster.Api.Data;
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
 public DbSet<Customer> Customers => Set<Customer>();
 public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
 protected override void OnModelCreating(ModelBuilder modelBuilder) {
  modelBuilder.Entity<Customer>().Property(x=>x.AmountTotal).HasPrecision(18,2);
  modelBuilder.Entity<Customer>().HasIndex(x=>x.Email);
 }
}
