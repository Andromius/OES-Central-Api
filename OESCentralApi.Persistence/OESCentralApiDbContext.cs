using Microsoft.EntityFrameworkCore;
using OESCentralApi.Domain.Organizations;
using System.Reflection;

namespace OESCentralApi.Persistence;

public class OESCentralApiDbContext : DbContext
{
    public DbSet<Organization> Organization { get; set; }
    public OESCentralApiDbContext() { }
    public OESCentralApiDbContext(DbContextOptions options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
