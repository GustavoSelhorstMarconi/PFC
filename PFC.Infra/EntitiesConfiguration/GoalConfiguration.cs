using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFC.Domain.Entities;

namespace PFC.Infra.EntitiesConfiguration;

public sealed class GoalConfiguration : IEntityTypeConfiguration<Goal>
{
    public void Configure(EntityTypeBuilder<Goal> builder)
    {
        builder.ToTable("Goals");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.TargetAmount).HasPrecision(18, 2);
        builder.Property(g => g.CurrentAmount).HasPrecision(18, 2).HasDefaultValue(0m);

        builder.HasIndex(g => g.UserId);
        builder.HasIndex(g => g.IsActive);

        builder.HasOne(g => g.User)
            .WithMany(u => u.Goals)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(g => g.Transactions)
            .WithOne(t => t.Goal)
            .HasForeignKey(t => t.GoalId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
