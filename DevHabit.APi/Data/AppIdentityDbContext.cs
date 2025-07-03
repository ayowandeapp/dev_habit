using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.APi.Data
{
    public sealed class AppIdentityDbContext(
        DbContextOptions<AppIdentityDbContext> options
    ): IdentityDbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser>().ToTable("asp_net_users");
            builder.Entity<IdentityRole>().ToTable("asp_net_roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("asp_net_user_roles");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("asp_net_role_claims");
            builder.Entity<IdentityUserClaim<string>>().ToTable("asp_net_user_claims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("asp_net_user_logins");
            builder.Entity<IdentityUserToken<string>>().ToTable("asp_net_user_tokens");
        }
    }
}