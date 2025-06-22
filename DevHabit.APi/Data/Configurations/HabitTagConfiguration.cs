using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHabit.APi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.APi.Data.Configurations
{
    public sealed class HabitTagConfiguration : IEntityTypeConfiguration<HabitTag>
    {
        public void Configure(EntityTypeBuilder<HabitTag> builder)
        {
            // Configure HabitId with max length 191 (to avoid MySQL key length limit)
            builder.Property(ht => ht.HabitId)
                .HasMaxLength(191)
                .IsRequired();
                
            // Configure TagId with max length 191
            builder.Property(ht => ht.TagId)
                .HasMaxLength(191)
                .IsRequired();

            builder.HasKey(ht => new { ht.HabitId, ht.TagId });
            builder
                .HasOne<Tag>()
                .WithMany()
                .HasForeignKey(ht => ht.TagId);
            builder
                .HasOne<Habit>()
                .WithMany(h => h.HashTags)
                .HasForeignKey(ht => ht.HabitId);


            //same as      
            // Configure foreign key to Tags       
            // builder
            //     .HasOne(ht => ht.Tag) //if we defined a navigation in HashTags entity
            //     .WithMany(t => t.HashTags)
            //     .HasForeignKey(ht => ht.TagId)
            //     .OnDelete(DeleteBehavior.NoAction);
            // Configure foreign key to Habits
            // builder
            //     .HasOne(ht => ht.Habit) //if we defined a navigation in HashTags entity
            //     .WithMany(h => h.HashTags)
            //     .HasForeignKey(ht => ht.HabitId)
            //     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}