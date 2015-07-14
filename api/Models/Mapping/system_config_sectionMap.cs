using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class system_config_sectionMap : EntityTypeConfiguration<system_config_section>
    {
        public system_config_sectionMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.nome)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.aplicacao)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("system_config_section");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.nome).HasColumnName("nome");
            this.Property(t => t.aplicacao).HasColumnName("aplicacao");
            this.Property(t => t.se_cliente).HasColumnName("se_cliente");
            this.Property(t => t.id_schema).HasColumnName("id_schema");
        }
    }
}
