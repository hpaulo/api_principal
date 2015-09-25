using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbCatalogoNewsGrupoMap : EntityTypeConfiguration<tbCatalogoNewsGrupo>
    {
        public tbCatalogoNewsGrupoMap()
        {
            // Primary Key
            this.HasKey(t => new { t.cdNewsGrupo, t.cdCatalogo });

            // Table & Column Mappings
            this.ToTable("tbCatalogoNewsGrupo", "admin");
            this.Property(t => t.cdNewsGrupo).HasColumnName("cdNewsGrupo");
            this.Property(t => t.cdCatalogo).HasColumnName("cdCatalogo");

            // Relationships
            this.HasRequired(t => t.tbCatalogo)
                .WithMany(t => t.tbCatalogoNewsGrupos)
                .HasForeignKey(d => d.cdCatalogo);
            this.HasRequired(t => t.tbNewsGrupo)
                .WithMany(t => t.tbCatalogoNewsGrupos)
                .HasForeignKey(d => d.cdNewsGrupo);

        }
    }
}