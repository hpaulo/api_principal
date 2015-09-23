using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbAssinanteMap : EntityTypeConfiguration<tbAssinante>
    {
        public tbAssinanteMap()
        {
            // Primary Key
            this.HasKey(t => new { t.cdNewsGrupo, t.cdUser });

            // Table & Column Mappings
            this.ToTable("tbAssinante", "admin");
            this.Property(t => t.cdNewsGrupo).HasColumnName("cdNewsGrupo");
            this.Property(t => t.cdUser).HasColumnName("cdUser");
            
            // Relationships
            this.HasRequired(t => t.webpages_Users)
                .WithMany(t => t.tbAssinantes)
                .HasForeignKey(d => d.cdUser);
            this.HasRequired(t => t.tbNewsGrupos)
                .WithMany(t => t.tbAssinantes)
                .HasForeignKey(d => d.cdNewsGrupo);

        }
    }
}
