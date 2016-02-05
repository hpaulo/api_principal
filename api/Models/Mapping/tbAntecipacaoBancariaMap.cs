using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbAntecipacaoBancariaMap : EntityTypeConfiguration<tbAntecipacaoBancaria>
    {
        public tbAntecipacaoBancariaMap()
        {
            // Primary Key
            this.HasKey(t => t.idAntecipacaoBancaria);

            this.Property(t => t.vlAntecipacao)
                .HasPrecision(9, 2)
                .IsRequired();

            this.Property(t => t.vlAntecipacaoLiquida)
                .HasPrecision(9, 2)
                .IsRequired();

            this.Property(t => t.dtVencimento)
                .IsRequired();

            this.Property(t => t.dtAntecipacaoBancaria)
                .IsRequired();

            this.Property(t => t.cdAdquirente)
                .IsRequired();

            this.Property(t => t.cdContaCorrente)
                .IsRequired();


            // Table & Column Mappings
            this.ToTable("tbAntecipacaoBancaria", "card");
            this.Property(t => t.idAntecipacaoBancaria).HasColumnName("idAntecipacaoBancaria");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.cdContaCorrente).HasColumnName("cdContaCorrente");
            this.Property(t => t.cdBandeira).HasColumnName("cdBandeira");
            this.Property(t => t.dtAntecipacaoBancaria).HasColumnName("dtAntecipacaoBancaria");
            this.Property(t => t.dtVencimento).HasColumnName("dtVencimento");
            this.Property(t => t.vlAntecipacao).HasColumnName("vlAntecipacao");
            this.Property(t => t.vlAntecipacaoLiquida).HasColumnName("vlAntecipacaoLiquida");

            // Relationships
            this.HasRequired(t => t.tbAdquirente)
                .WithMany(t => t.tbAntecipacaoBancarias)
                .HasForeignKey(d => d.cdAdquirente);
            this.HasRequired(t => t.tbContaCorrente)
                .WithMany(t => t.tbAntecipacaoBancarias)
                .HasForeignKey(d => d.cdContaCorrente);
            this.HasOptional(t => t.tbBandeira)
                .WithMany(t => t.tbAntecipacaoBancarias)
                .HasForeignKey(d => d.cdBandeira);
        }
    }
}