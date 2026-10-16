using AcxiomCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers => Set<Customer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.CustomerId);

                entity.Property(c => c.CustomerCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.CustomerName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(c => c.Phone)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(c => c.CompanyName)
                    .HasMaxLength(150);

                entity.Property(c => c.Address)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(c => c.City)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.State)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Active");

                entity.Property(c => c.CreatedDate)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(c => c.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValue("System");

                // Database-level unique constraints
                entity.HasIndex(c => c.CustomerCode)
                    .IsUnique()
                    .HasDatabaseName("IX_Customers_CustomerCode");

                entity.HasIndex(c => c.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Customers_Email");

                entity.HasIndex(c => c.Phone)
                    .IsUnique()
                    .HasDatabaseName("IX_Customers_Phone");
            });
        }
    }
}
