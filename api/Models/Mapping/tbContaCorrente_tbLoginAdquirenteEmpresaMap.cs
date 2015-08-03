using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbContaCorrente_tbLoginAdquirenteEmpresaMap : EntityTypeConfiguration<tbContaCorrente_tbLoginAdquirenteEmpresa>
    {
        public tbContaCorrente_tbLoginAdquirenteEmpresaMap()
        {
            // Primary Key
            this.HasKey(t => new { t.cdContaCorrente, t.cdLoginAdquirenteEmpresa });

            // Properties
            this.Property(t => t.cdContaCorrente)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.cdLoginAdquirenteEmpresa)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("tbContaCorrente_tbLoginAdquirenteEmpresa", "card");
            this.Property(t => t.cdContaCorrente).HasColumnName("cdContaCorrente");
            this.Property(t => t.cdLoginAdquirenteEmpresa).HasColumnName("cdLoginAdquirenteEmpresa");
            this.Property(t => t.dtInicio).HasColumnName("dtInicio");
            this.Property(t => t.dtFim).HasColumnName("dtFim");

            // Relationships
            this.HasRequired(t => t.tbContaCorrente)
                .WithMany(t => t.tbContaCorrente_tbLoginAdquirenteEmpresa)
                .HasForeignKey(d => d.cdContaCorrente);

        }
    }
}