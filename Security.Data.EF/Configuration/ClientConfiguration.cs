using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain;

namespace Security.Data.EF.Configuration
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");

            // Primary Key
            builder.HasKey(c => c.ClientId);

            // Properties
            builder.Property(c => c.ClientId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(c => c.ClientName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ApiKeyHash)
                .IsRequired()
                .HasColumnType("varbinary(128)");

            builder.Property(c => c.ApiKeySalt)
                .IsRequired()
                .HasColumnType("varbinary(32)");

            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2(3)")
                .HasDefaultValueSql("GETUTCDATE()");

            // Unique index for client name
            builder.HasIndex(c => c.ClientName)
                .IsUnique();
        }
    }

}
