using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFC.Domain.Entities;

namespace PFC.Infra.EntitiesConfiguration;

public sealed class DebtConfiguration : IEntityTypeConfiguration<Debt>
{
    public void Configure(EntityTypeBuilder<Debt> builder)
    {
        builder.ToTable("Debts");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(d => d.RemainingAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(d => d.InterestRate)
            .HasPrecision(5, 2);

        builder.Property(d => d.DueDate);

        builder.Property(d => d.IsActive)
            .IsRequired();

        builder.HasIndex(d => d.UserId);
        builder.HasIndex(d => new { d.UserId, d.IsActive });

        builder.HasOne(d => d.User)
            .WithMany(u => u.Debts)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Transactions)
            .WithOne(t => t.Debt)
            .HasForeignKey(t => t.DebtId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
