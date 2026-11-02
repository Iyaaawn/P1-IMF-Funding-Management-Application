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

        modelBuilder.Entity<LendingFacility>().HasData(
            new LendingFacility { Id = 1, CodeName = "SBA", FullName = "Stand-By Arrangement",
                Description = "Short-term support for countries with balance-of-payments problems.", MaxTermMonths = 36 },
            new LendingFacility { Id = 2, CodeName = "EFF", FullName = "Extended Fund Facility",
                Description = "Longer-term support for countries with deep structural problems.", MaxTermMonths = 48 },
            new LendingFacility { Id = 3, CodeName = "ECF", FullName = "Extended Credit Facility",
                Description = "Concessional (low-interest) support for low-income countries.", MaxTermMonths = 60 },
            new LendingFacility { Id = 4, CodeName = "FCL", FullName = "Flexible Credit Line",
                Description = "Precautionary credit line for countries with very strong policies.", MaxTermMonths = 24 },
            new LendingFacility { Id = 5, CodeName = "RSF", FullName = "Resilience and Sustainability Facility",
                Description = "Long-term financing for climate and pandemic resilience.", MaxTermMonths = 36 }
        );

        modelBuilder.Entity<Country>().HasData(
            new Country { Id = 1, Name = "Ghana",     IsoCode = "GHA", QuotaAmountSdrMillions = 738.0m },
            new Country { Id = 2, Name = "Kenya",     IsoCode = "KEN", QuotaAmountSdrMillions = 542.8m },
            new Country { Id = 3, Name = "Argentina", IsoCode = "ARG", QuotaAmountSdrMillions = 3187.3m },
            new Country { Id = 4, Name = "Egypt",     IsoCode = "EGY", QuotaAmountSdrMillions = 2037.1m },
            new Country { Id = 5, Name = "Pakistan",  IsoCode = "PAK", QuotaAmountSdrMillions = 2031.0m },
            new Country { Id = 6, Name = "Sri Lanka", IsoCode = "LKA", QuotaAmountSdrMillions = 578.8m }
        );
    }
}