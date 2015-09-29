using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbAssinanteMap : EntityTypeConfiguration<tbAssinante>
    {
        public tbAssinanteMap()
        {
            // Primary Key
            this.HasKey(t => new { t.cdCatalogo, t.id_users });

            // Table & Column Mappings
            this.ToTable("tbAssinante", "admin");
            this.Property(t => t.cdCatalogo).HasColumnName("cdCatalogo");
            this.Property(t => t.id_users).HasColumnName("id_users");
            
            // Relationships
            this.HasRequired(t => t.webpages_Users)
                .WithMany(t => t.tbAssinantes)
                .HasForeignKey(d => d.id_users);
            this.HasRequired(t => t.tbCatalogo)
                .WithMany(t => t.tbAssinantes)
                .HasForeignKey(d => d.cdCatalogo);

        }
    }
}
