using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Cosmos;

using ProxyVote.Core.Entities;

namespace ProxyVote.Core.Adapters;

public class ProxyDBContext : DbContext
{
    /// <summary>
    /// Name of the partition key shadow property.
    /// </summary>
    public const string PartitionKey = nameof(PartitionKey);

    public DbSet<ProxyRegistration> ProxyRegistrations  { get; set; }

    //public void SetPartitionKey<T>(T entity)
    //where T : class, IDocSummaries =>
    //    Entry(entity).Property(PartitionKey).CurrentValue =
    //        ComputePartitionKey<T>();


    public ProxyDBContext(DbContextOptions<ProxyDBContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var registrationModel = modelBuilder.Entity<ProxyRegistration>();

        registrationModel.Property<string>(PartitionKey);

        registrationModel
            .ToContainer("Registrations")
            .HasPartitionKey(PartitionKey)
            .HasKey(r => r.RegistrationId);
        

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Set the partition key shadow property.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="entity">The entity.</param>
    public void SetPartitionKey<T>(T entity, string value) where T : class =>
        Entry(entity).Property(PartitionKey).CurrentValue = value;

    
}
