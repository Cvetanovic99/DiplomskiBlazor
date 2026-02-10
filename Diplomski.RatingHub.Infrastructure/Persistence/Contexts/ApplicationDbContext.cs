using Diplomski.RatingHub.Domain.Models;
using Diplomski.RatingHub.Infrastructure.Auth.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diplomski.RatingHub.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryKeyword> CategoryKeywords { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyImage> CompanyImages { get; set; }
    public DbSet<CompanyRatingAggregate> CompanyRatingAggregates { get; set; }
    public DbSet<CompanyResponse> CompanyResponses { get; set; }
    public DbSet<CompanyResponseImage> CompanyResponseImages { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<NewCategorySuggestion> NewCategorySuggestions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<RatingCriterion> RatingCriteria { get; set; }
    public DbSet<ReportedContent> ReportedContents { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ReviewGrade> ReviewGrades { get; set; }
    public DbSet<ReviewImage> ReviewImages { get; set; }
    public DbSet<UserImage> UserImages { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        HandleAuditFieldsBeforeSaveChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ConfigureCategory(EntityTypeBuilder<Category> builder)
    {
        
    }

    private void HandleAuditFieldsBeforeSaveChanges()
    {
        var changedEntities = ChangeTracker.Entries<EntityBase>()
            .Where(e => e.State != EntityState.Unchanged).ToList();

        foreach (var entry in changedEntities)
        {
            var utcNow = DateTime.UtcNow;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Created = utcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.Modified = utcNow;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}