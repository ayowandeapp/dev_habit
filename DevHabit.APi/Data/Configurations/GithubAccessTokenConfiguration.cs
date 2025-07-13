using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.APi.Data.Configurations
{
    public sealed class GithubAccessTokenConfiguration : IEntityTypeConfiguration<GitHubAccessToken>
    {
        public void Configure(EntityTypeBuilder<GitHubAccessToken> builder)
        {
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Id).HasMaxLength(500);
            builder.Property(g => g.UserId).HasMaxLength(500);
            builder.Property(g => g.Token).HasMaxLength(500);

            builder.HasIndex(g => g.UserId);

            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<GitHubAccessToken>(g => g.UserId);
        }
    }
}