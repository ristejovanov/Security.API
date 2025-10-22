using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain;

namespace Security.Data.EF.Configuration
{ 
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            // Primary Key
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.MobileNumber)
                .HasMaxLength(20);

            builder.Property(u => u.Language)
                .HasMaxLength(10);

            builder.Property(u => u.Culture)
                .HasMaxLength(10);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasColumnType("varbinary(128)");

            builder.Property(u => u.PasswordSalt)
                .IsRequired()
                .HasColumnType("varbinary(32)");

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2(3)")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.UpdatedAt)
                .HasColumnType("datetime2(3)")
                .IsRequired(false);

            // Unique constraints
            builder.HasIndex(u => u.UserName).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}


