using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbRecebimentoTEFMap : EntityTypeConfiguration<tbRecebimentoTEF>
    {
        public tbRecebimentoTEFMap()
        {
            // Primary Key
            this.HasKey(t => t.idRecebimentoTEF);

            // Properties
            this.Property(t => t.nrCNPJ)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(14);

            this.Property(t => t.cdEstabelecimentoTEF)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.nrPDVTEF)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.nrNSU)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.cdAutorizacao)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.nrCartao)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.nmOperadora)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("tbRecebimentoTEF", "card");
            this.Property(t => t.idRecebimentoTEF).HasColumnName("idRecebimentoTEF");
            this.Property(t => t.cdGrupo).HasColumnName("cdGrupo");
            this.Property(t => t.nrCNPJ).HasColumnName("nrCNPJ");
            this.Property(t => t.cdEstabelecimentoTEF).HasColumnName("cdEstabelecimentoTEF");
            this.Property(t => t.nrPDVTEF).HasColumnName("nrPDVTEF");
            this.Property(t => t.nrNSU).HasColumnName("nrNSU");
            this.Property(t => t.cdAutorizacao).HasColumnName("cdAutorizacao");
            this.Property(t => t.cdSitef).HasColumnName("cdSitef");
            this.Property(t => t.dtVenda).HasColumnName("dtVenda");
            this.Property(t => t.hrVenda).HasColumnName("hrVenda");
            this.Property(t => t.vlVenda).HasColumnName("vlVenda");
            this.Property(t => t.nrCartao).HasColumnName("nrCartao");
            this.Property(t => t.cdBandeira).HasColumnName("cdBandeira");
            this.Property(t => t.nmOperadora).HasColumnName("nmOperadora");
            this.Property(t => t.dthrVenda).HasColumnName("dthrVenda");
        }
    }
}
