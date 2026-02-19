namespace Persistance.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(e => e.Email)
                .IsUnique();

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.PasswordHash)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.RefreshToken)
                .HasMaxLength(500);
            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
