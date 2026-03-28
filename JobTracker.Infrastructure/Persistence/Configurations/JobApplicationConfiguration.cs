using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Infrastructure.Persistence.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.JobTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.JobUrl)
            .HasMaxLength(2000);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Notes)
            .HasMaxLength(5000);

        builder.Property(x => x.CvFileName)
            .HasMaxLength(500);

        builder.Property(x => x.CvFilePath)
            .HasMaxLength(1000);

        // FK to AspNetUsers - no navigation property on domain entity
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasMany(x => x.Interviews)
            .WithOne(i => i.JobApplication)
            .HasForeignKey(i => i.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.UserId, x.Status });
    }
}
