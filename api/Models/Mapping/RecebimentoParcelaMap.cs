using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class RecebimentoParcelaMap : EntityTypeConfiguration<RecebimentoParcela>
    {
        public RecebimentoParcelaMap()
        {
            // Primary Key
            this.HasKey(t => new { t.idRecebimento, t.numParcela });

            // Properties
            this.Property(t => t.dtaRecebimento)
                .IsRequired();

            this.Property(t => t.valorParcelaBruta)
               .HasPrecision(9, 3)
               .IsRequired();

            this.Property(t => t.valorDescontado)
                .HasPrecision(9, 3)
                .IsRequired();

            this.Property(t => t.valorParcelaLiquida)
                .HasPrecision(9, 4)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            this.Property(t => t.vlDescontadoAntecipacao)
                .HasPrecision(9, 4)
                .IsRequired();

            this.Property(t => t.flAntecipado)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("RecebimentoParcela", "pos");
            this.Property(t => t.idRecebimento).HasColumnName("idRecebimento");
            this.Property(t => t.numParcela).HasColumnName("numParcela");
            this.Property(t => t.valorParcelaBruta).HasColumnName("valorParcelaBruta");
            this.Property(t => t.valorParcelaLiquida).HasColumnName("valorParcelaLiquida");
            this.Property(t => t.dtaRecebimento).HasColumnName("dtaRecebimento");
            this.Property(t => t.valorDescontado).HasColumnName("valorDescontado");
            this.Property(t => t.idExtrato).HasColumnName("idExtrato");
            this.Property(t => t.dtaRecebimentoEfetivo).HasColumnName("dtaRecebimentoEfetivo");
            this.Property(t => t.vlDescontadoAntecipacao).HasColumnName("vlDescontadoAntecipacao");
            this.Property(t => t.idRecebimentoTitulo).HasColumnName("idRecebimentoTitulo"); 
            this.Property(t => t.flAntecipado).HasColumnName("flAntecipado");
            this.Property(t => t.idAntecipacaoBancariaDetalhe).HasColumnName("idAntecipacaoBancariaDetalhe");

            // Relationships
            this.HasRequired(t => t.Recebimento)
                .WithMany(t => t.RecebimentoParcelas)
                .HasForeignKey(d => d.idRecebimento);
            this.HasOptional(t => t.tbExtrato)
                .WithMany(t => t.RecebimentoParcelas)
                .HasForeignKey(d => d.idExtrato);
            this.HasOptional(t => t.tbRecebimentoTitulo)
                .WithMany(t => t.RecebimentoParcelas)
                .HasForeignKey(d => d.idRecebimentoTitulo);
            this.HasOptional(t => t.tbAntecipacaoBancariaDetalhe)
                .WithMany(t => t.RecebimentoParcelas)
                .HasForeignKey(d => d.idAntecipacaoBancariaDetalhe);
        }
    }
}
