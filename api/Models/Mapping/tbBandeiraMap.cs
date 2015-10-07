using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbBandeiraMap : EntityTypeConfiguration<tbBandeira>
    {
        public tbBandeiraMap()
        {
            // Primary Key
            this.HasKey(t => t.cdBandeira);

            // Properties
            this.Property(t => t.dsBandeira)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.cdAdquirente);

            this.Property(t => t.dsTipo)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("tbBandeira", "card");
            this.Property(t => t.cdBandeira).HasColumnName("cdBandeira");
            this.Property(t => t.dsBandeira).HasColumnName("dsBandeira");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.dsTipo).HasColumnName("dsTipo");

            // Relationships
            this.HasRequired(t => t.tbAdquirente)
                .WithMany(t => t.tbBandeiras)
                .HasForeignKey(d => d.cdAdquirente);
        }
    }
}

