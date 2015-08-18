using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbEmpresaMap : EntityTypeConfiguration<tbEmpresa>
    {
        public tbEmpresaMap()
        {
            // Primary Key
            this.HasKey(t => t.nrCNPJBase);

            // Properties
            this.Property(t => t.nrCNPJBase)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.dsCertificadoDigital)
                .HasMaxLength(1000);

            this.Property(t => t.dsCertificadoDigitalSenha)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("tbEmpresa", "admin");
            this.Property(t => t.nrCNPJBase).HasColumnName("nrCNPJBase");
            this.Property(t => t.dsCertificadoDigital).HasColumnName("dsCertificadoDigital");
            this.Property(t => t.dsCertificadoDigitalSenha).HasColumnName("dsCertificadoDigitalSenha");
            this.Property(t => t.cdEmpresaGrupo).HasColumnName("cdEmpresaGrupo");

            // Relationships
            this.HasRequired(t => t.tbEmpresaGrupo)
                .WithMany(t => t.tbEmpresas)
                .HasForeignKey(d => d.cdEmpresaGrupo);

        }
    }
}
