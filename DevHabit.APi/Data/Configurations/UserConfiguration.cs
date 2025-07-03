using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.APi.Data.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasMaxLength(500);
            builder.Property(u => u.Email).HasMaxLength(300);
            builder.Property(u => u.Name).HasMaxLength(500);

            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.identityId).IsUnique();
        }
    }
}