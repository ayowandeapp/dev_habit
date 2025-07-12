using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.APi.Data.Configurations
{
    public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasMaxLength(500);
            builder.Property(h => h.UserId).HasMaxLength(500);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Description).HasMaxLength(500);
            builder.HasIndex(t => new { t.Name, t.UserId }).IsUnique();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(h => h.UserId);
        }
    }
}