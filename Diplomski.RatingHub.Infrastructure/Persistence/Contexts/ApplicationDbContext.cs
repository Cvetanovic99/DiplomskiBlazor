using Diplomski.RatingHub.Domain.Models;
using Diplomski.RatingHub.Infrastructure.Auth.Models;
using Diplomski.RatingHub.Infrastructure.Persistence.Extensions;
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
        modelBuilder.Entity<UserProfile>(ConfigureUserProfile);
        modelBuilder.Entity<ReportedContent>(ConfigureReportedContent);
        modelBuilder.Entity<Company>(ConfigureCompany);
        modelBuilder.Entity<Notification>(ConfigureNotification);
        modelBuilder.Entity<CompanyRatingAggregate>(ConfigureCompanyRatingAggregate);
        modelBuilder.Entity<Review>(ConfigureReview);
        modelBuilder.Entity<ReviewGrade>(ConfigureReviewGrade);
        modelBuilder.Entity<Category>(ConfigureCategory);
        modelBuilder.Entity<RatingCriterion>(ConfigureRatingCriterion);
        
        modelBuilder.SeedCities();
        modelBuilder.SeedIdentityRoles();
        
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        HandleAuditFieldsBeforeSaveChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    
    private void ConfigureUserProfile(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasOne<UserImage>()
            .WithOne(x => x.User)
            .HasForeignKey<UserImage>(i => i.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureReportedContent(EntityTypeBuilder<ReportedContent> builder)
    {
        builder.HasOne(r => r.ReporterUser)
            .WithMany()
            .HasForeignKey(r => r.ReporterUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ReportedUser)
            .WithMany()
            .HasForeignKey(r => r.ReportedUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureCompany(EntityTypeBuilder<Company> builder)
    {
        builder.HasOne(c => c.Creator)
            .WithMany(u => u.CreatedCompanies)
            .HasForeignKey(c => c.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(c => c.Owner)
            .WithMany(u => u.OwningCompanies)
            .HasForeignKey(c => c.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(co => co.City)
            .WithMany(c => c.Companies)
            .HasForeignKey(c => c.CityId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(co => co.Category)
            .WithMany(c => c.Companies)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureNotification(EntityTypeBuilder<Notification> builder)
    {
        builder.HasOne(n => n.Recipient)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.RecipientId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(n => n.Actor)
            .WithMany()
            .HasForeignKey(n => n.ActorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
    private void ConfigureCompanyRatingAggregate(EntityTypeBuilder<CompanyRatingAggregate> builder)
    {
        builder.HasIndex(cr => new { cr.CompanyId, cr.RatingCriterionId })
            .IsUnique();
        
        //Not need for explicit configuration in fluent API, EF Core convention would work but explicit is better
        builder.HasOne(cr => cr.RatingCriterion)
            .WithMany()
            .HasForeignKey(cr => cr.RatingCriterionId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureReview(EntityTypeBuilder<Review> builder)
    {
        builder.HasOne<CompanyResponse>()
            .WithOne(cr => cr.Review)
            .HasForeignKey<CompanyResponse>(cr => cr.ReviewId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(r => r.Company)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(r => r.Reviewer)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureReviewGrade(EntityTypeBuilder<ReviewGrade> builder)
    {
        builder.HasIndex(rg => new { rg.ReviewId, rg.RatingCriterionId })
            .IsUnique();

        //Not need for explicit configuration in fluent API, EF Core convention would work but explicit is better
        builder.HasOne(rg => rg.RatingCriterion)
            .WithMany()
            .HasForeignKey(rg => rg.RatingCriterionId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureCategory(EntityTypeBuilder<Category> builder)
    {
        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureRatingCriterion(EntityTypeBuilder<RatingCriterion> builder)
    {
        builder.HasIndex(rc => new { rc.CategoryId, rc.Name })
            .IsUnique();
    }

    private void ConfigureCategoryKeyword(EntityTypeBuilder<CategoryKeyword> builder)
    {
        //builder.HasIndex(k => new {k.CategoryId, k.Keyword})
        //    .IsUnique();
        builder.HasIndex(k => k.Keyword);
    }
    
    private void ConfigureNewCategorySuggestion(EntityTypeBuilder<NewCategorySuggestion> builder)
    {
        //Not need for explicit configuration in fluent API, EF Core convention would work but explicit is better
        builder.HasOne(nc => nc.ParentCategory)
            .WithMany()
            .HasForeignKey(nc => nc.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
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