using Microsoft.EntityFrameworkCore;
using ImfFunding.Api.Models.EF;

namespace ImfFunding.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries { get; set; }
    public DbSet<LendingFacility> LendingFacilities { get; set; }
    public DbSet<FundingRequest> FundingRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Store the Status enum as text ("Submitted") instead of a number (0)
        modelBuilder.Entity<FundingRequest>()
            .Property(r => r.Status)
            .HasConversion<string>();
    }
}