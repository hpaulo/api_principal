using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbTerminalLogicoMap : EntityTypeConfiguration<tbTerminalLogico>
    {
        public tbTerminalLogicoMap()
        {
            // Primary Key
            this.HasKey(t => t.cdTerminalLogico);

            // Properties
            this.Property(t => t.cdTerminalLogico)
                .HasMaxLength(20);

            this.Property(t => t.cdAdquirente)
                .IsRequired();

            this.Property(t => t.nrCNPJ)
                .IsFixedLength()
                .IsRequired()
                .HasMaxLength(14);

            // Table & Column Mappings
            this.ToTable("tbTerminalLogico", "card");
            this.Property(t => t.cdTerminalLogico).HasColumnName("cdTerminalLogico");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.nrCNPJ).HasColumnName("nrCNPJ");

            // Relationships
            this.HasRequired(t => t.tbAdquirente)
                .WithMany(t => t.tbTerminalLogicos)
                .HasForeignKey(t => t.cdAdquirente);
            this.HasRequired(t => t.empresa)
                .WithMany(t => t.tbTerminalLogicos)
                .HasForeignKey(t => t.nrCNPJ);

        }
    }
}
