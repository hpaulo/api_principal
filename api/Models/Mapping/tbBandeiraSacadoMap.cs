﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbBandeiraSacadoMap : EntityTypeConfiguration<tbBandeiraSacado>
    {
        public tbBandeiraSacadoMap()
        {
            // Primary Key
            this.HasKey(t => new { t.cdGrupo, t.cdBandeira, t.qtParcelas });

            // Properties
            this.Property(t => t.cdSacado)
                .HasMaxLength(10)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("tbBandeiraSacado", "card");
            this.Property(t => t.cdGrupo).HasColumnName("cdGrupo");
            this.Property(t => t.cdSacado).HasColumnName("cdSacado");
            this.Property(t => t.cdBandeira).HasColumnName("cdBandeira");
            this.Property(t => t.qtParcelas).HasColumnName("qtParcelas");

            // Relationships
            this.HasRequired(t => t.grupo_empresa)
                .WithMany(t => t.tbBandeiraSacados)
                .HasForeignKey(d => d.cdGrupo);
            this.HasRequired(t => t.tbBandeira)
                .WithMany(t => t.tbBandeiraSacados)
                .HasForeignKey(d => d.cdBandeira);


        }
    }
}
