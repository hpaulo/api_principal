using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class config_entriesMap : EntityTypeConfiguration<config_entries>
    {
        public config_entriesMap()
        {
            // Primary Key
            this.HasKey(t => new { t.chave, t.section_id });

            // Properties
            this.Property(t => t.chave)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.valor)
                .HasMaxLength(255);

            this.Property(t => t.section_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("config_entries");
            this.Property(t => t.chave).HasColumnName("chave");
            this.Property(t => t.valor).HasColumnName("valor");
            this.Property(t => t.section_id).HasColumnName("section_id");

            // Relationships
            this.HasRequired(t => t.config_section)
                .WithMany(t => t.config_entries)
                .HasForeignKey(d => d.section_id);

        }
    }
}
