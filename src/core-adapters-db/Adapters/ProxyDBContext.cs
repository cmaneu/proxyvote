using Microsoft.EntityFrameworkCore;
using ProxyVote.Core.Entities;

namespace ProxyVote.Core.Adapters;

public class ProxyDBContext : DbContext
{
    /// <summary>
    /// Name of the partition key shadow property.
    /// </summary>
    public const string PartitionKey = nameof(PartitionKey);

    public DbSet<ProxyApplication?> ProxyRegistrations  { get; set; }

    //public void SetPartitionKey<T>(T entity)
    //where T : class, IDocSummaries =>
    //    Entry(entity).Property(PartitionKey).CurrentValue =
    //        ComputePartitionKey<T>();


    public ProxyDBContext(DbContextOptions<ProxyDBContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var applicationModel = modelBuilder.Entity<ProxyApplication>();
        applicationModel.Property<string>(PartitionKey);
        applicationModel
            .ToContainer("Applications")
            //.HasNoDiscriminator()
            .HasPartitionKey(PartitionKey)
            .HasKey(r => r.RegistrationId);
        

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Set the partition key shadow property.
    /// </summary>
    public void SetPartitionKey<T>(T entity, string value) where T : class =>
        Entry(entity).Property(PartitionKey).CurrentValue = value;

    
}
