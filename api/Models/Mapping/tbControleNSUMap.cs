using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace api.Models.Mapping
{
    public class tbControleNSUMap : EntityTypeConfiguration<tbControleNSU>
    {
        public tbControleNSUMap()
        { 
                    // Primary Key
            this.HasKey(t => t.idControle);

            // Properties
            this.Property(t => t.nrCNPJ)
                .IsRequired();

            this.Property(t => t.ultNSU)
              .IsRequired();

            this.Property(t => t.nrCNPJ)
                .HasMaxLength(14);

            this.Property(t => t.ultNSU)
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("tbControleNSU", "tax");
            this.Property(t => t.idControle).HasColumnName("idControle");
            this.Property(t => t.nrCNPJ).HasColumnName("nrCNPJ");
            this.Property(t => t.ultNSU).HasColumnName("ultNSU");
        
        }
    }
}