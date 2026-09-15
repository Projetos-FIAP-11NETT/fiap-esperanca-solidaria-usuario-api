using FiapEsperancaSolidaria.Usuario.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiapEsperancaSolidaria.Usuario.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(nameof(Role), "identity");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .HasMaxLength(10);
        builder.HasData(
            new Role(1, "GestorONG"),
            new Role(2, "Doador"));
    }
}
