using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbAntecipacaoBancariaDetalheMap : EntityTypeConfiguration<tbAntecipacaoBancariaDetalhe>
    {
        public tbAntecipacaoBancariaDetalheMap()
        {
            // Primary Key
            this.HasKey(t => t.idAntecipacaoBancariaDetalhe);

            this.Property(t => t.idAntecipacaoBancaria)
                .IsRequired();

            this.Property(t => t.vlAntecipacao)
                .HasPrecision(9, 2)
                .IsRequired();

            this.Property(t => t.vlAntecipacaoLiquida)
                .HasPrecision(9, 2)
                .IsRequired();

            this.Property(t => t.dtVencimento)
                .IsRequired();


            // Table & Column Mappings
            this.ToTable("tbAntecipacaoBancariaDetalhe", "card");
            this.Property(t => t.idAntecipacaoBancariaDetalhe).HasColumnName("idAntecipacaoBancariaDetalhe");
            this.Property(t => t.idAntecipacaoBancaria).HasColumnName("idAntecipacaoBancaria");
            this.Property(t => t.cdBandeira).HasColumnName("cdBandeira");
            this.Property(t => t.dtVencimento).HasColumnName("dtVencimento");
            this.Property(t => t.vlAntecipacao).HasColumnName("vlAntecipacao");
            this.Property(t => t.vlAntecipacaoLiquida).HasColumnName("vlAntecipacaoLiquida");

            // Relationships
            this.HasRequired(t => t.tbAntecipacaoBancaria)
                .WithMany(t => t.tbAntecipacaoBancariaDetalhes)
                .HasForeignKey(d => d.idAntecipacaoBancaria);
            this.HasOptional(t => t.tbBandeira)
                .WithMany(t => t.tbAntecipacaoBancariaDetalhes)
                .HasForeignKey(d => d.cdBandeira);
        }
    }
}