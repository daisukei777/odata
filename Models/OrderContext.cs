using Microsoft.EntityFrameworkCore;

namespace odata.Models
{
    public class OrderContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Distributor> Distributors { get; set; }

        readonly string accountEndpoint = "";
        readonly string accountKey = "";
        readonly string databaseName = "";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseCosmos(accountEndpoint, accountKey, databaseName);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region DefaultContainer
            modelBuilder.HasDefaultContainer("Store");
            #endregion

            #region Container
            modelBuilder.Entity<Order>()
                .ToContainer("Orders");
            #endregion

            #region NoDiscriminator
            modelBuilder.Entity<Order>()
                .HasNoDiscriminator();
            #endregion

            #region PartitionKey
            modelBuilder.Entity<Order>()
                .HasPartitionKey(o => o.PartitionKey);
            #endregion

            #region ETag
            modelBuilder.Entity<Order>()
                .UseETagConcurrency();
            #endregion

            #region PropertyNames
            modelBuilder.Entity<Order>().OwnsOne(
                o => o.ShippingAddress,
                sa =>
                {
                    sa.ToJsonProperty("Address");
                    sa.Property(p => p.Street).ToJsonProperty("ShipsToStreet");
                    sa.Property(p => p.City).ToJsonProperty("ShipsToCity");
                });
            #endregion

            #region OwnsMany
            modelBuilder.Entity<Distributor>().OwnsMany(p => p.ShippingCenters);
            #endregion

            #region ETagProperty
            modelBuilder.Entity<Distributor>()
                .Property(d => d.ETag)
                .IsETagConcurrency();
            #endregion
        }
    }
}
