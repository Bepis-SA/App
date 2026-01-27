using Bepixplore.Destinations;
using Bepixplore.Experiences;
using Bepixplore.Favorites;
using Bepixplore.Ratings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq.Expressions;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Users;

namespace Bepixplore.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class BepixploreDbContext :
    AbpDbContext<BepixploreDbContext>,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<Destination> Destinations { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    public DbSet<TravelExperience> TravelExperiences { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext 
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext .
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    #endregion

    public BepixploreDbContext(DbContextOptions<BepixploreDbContext> options)
        : base(options)
    {
    }

    private ICurrentUser CurrentUser => LazyServiceProvider.LazyGetRequiredService<ICurrentUser>();
    protected bool IsUserOwnedFilterEnabled => DataFilter?.IsEnabled<IUserOwned>() ?? false;
    protected override bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType)
    {
        if (typeof(IUserOwned).IsAssignableFrom(typeof(TEntity)))
        {
            return true;
        }
        return base.ShouldFilterEntity<TEntity>(entityType);
    }

    protected override Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>(ModelBuilder modelBuilder)
    {
        var expression = base.CreateFilterExpression<TEntity>(modelBuilder);

        if (typeof(IUserOwned).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> userOwnedFilter =
                e => !IsUserOwnedFilterEnabled ||
                     EF.Property<Guid>(e, nameof(IUserOwned.UserId)) == CurrentUser.Id.GetValueOrDefault();

            expression = expression == null
                ? userOwnedFilter
                : QueryFilterExpressionHelper.CombineExpressions(expression, userOwnedFilter);
        }

        return expression;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(BepixploreConsts.DbTablePrefix + "YourEntities", BepixploreConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

        builder.Entity<Destination>(b =>
        {
            b.ToTable("Destinations");
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Country).IsRequired().HasMaxLength(100);
            b.Property(x => x.City).IsRequired().HasMaxLength(100);
            b.Property(x => x.Population).IsRequired();
            b.Property(x => x.Photo).HasMaxLength(500);
            b.Property(x => x.UpdateDate).IsRequired();
            b.OwnsOne(d => d.Coordinates, co =>
            {
                co.Property(c => c.Latitude).HasColumnName("Latitude").IsRequired().HasColumnType("float");
                co.Property(c => c.Longitude).HasColumnName("Longitude").IsRequired().HasColumnType("float");
            });
        });

        builder.Entity<Rating>(b =>
        {
            b.ToTable("Ratings");
            b.ConfigureByConvention();
            b.Property(x => x.Score).IsRequired().HasMaxLength(5);
            b.Property(x => x.Comment).HasMaxLength(500);
            b.Property(x => x.DestinationId).IsRequired();
            b.HasOne<Destination>().WithMany().HasForeignKey(x => x.DestinationId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Favorite>(b =>
        {
            b.ToTable("Favorites"); // Nombre de la tabla
            b.ConfigureByConvention(); // Configura las auditorías de ABP
                                       // Índice único para evitar que un usuario agregue dos veces el mismo destino
            b.HasIndex(x => new { x.UserId, x.DestinationId }).IsUnique();
        });

        builder.Entity<TravelExperience>(b => 
        { 
            b.ToTable("TravelExperiences"); 
            b.ConfigureByConvention(); 
        });
    }
}

