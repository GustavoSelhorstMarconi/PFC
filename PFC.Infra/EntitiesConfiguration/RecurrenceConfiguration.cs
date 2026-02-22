using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFC.Domain.Entities;
using PFC.Domain.Enums;

namespace PFC.Infra.EntitiesConfiguration;

public sealed class RecurrenceConfiguration : IEntityTypeConfiguration<Recurrence>
{
    public void Configure(EntityTypeBuilder<Recurrence> builder)
    {
        builder.ToTable("recurrences");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Amount).HasPrecision(18, 2);
        builder.Property(r => r.Frequency).HasConversion<string>().HasMaxLength(50);

        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.IsActive);

        builder.HasOne(r => r.Account)
            .WithMany()
            .HasForeignKey(r => r.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Category)
            .WithMany()
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
