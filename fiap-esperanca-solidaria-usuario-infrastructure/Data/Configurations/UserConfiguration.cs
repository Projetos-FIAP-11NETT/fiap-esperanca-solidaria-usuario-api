using FiapEsperancaSolidaria.Usuario.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiapEsperancaSolidaria.Usuario.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User), "identity");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(x => x.FirebaseUserId)
            .HasMaxLength(30);
        builder.Property(x => x.CPF)
            .IsRequired()
            .HasMaxLength(11)
            .HasColumnType("char(11)");
        builder.Property(x => x.Image)
            .IsRequired(false);
        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity<Dictionary<string, object>>("UserRoles",
                e => e
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("RoleId"),
                e =>
                    e.HasOne<User>()
                        .WithMany()
                        .HasForeignKey("UserId"),
                e => e.ToTable("UserRoles", "identity"));
    }
}
