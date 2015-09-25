using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbCatalogoMap : EntityTypeConfiguration<tbCatalogo>
    {
        public tbCatalogoMap()
        {
            // Primary Key
            this.HasKey(t => t.cdCatalogo);

            // Properties
            this.Property(t => t.dsCatalogo)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tbCatalogo", "admin");
            this.Property(t => t.cdCatalogo).HasColumnName("cdCatalogo");
            this.Property(t => t.dsCatalogo).HasColumnName("dsCatalogo");            
        }
    }
}