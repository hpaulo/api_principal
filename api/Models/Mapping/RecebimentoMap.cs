using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class RecebimentoMap : EntityTypeConfiguration<Recebimento>
    {
        public RecebimentoMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.cnpj)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(14);

            this.Property(t => t.valorVendaBruta)
                .HasPrecision(9, 3)
                .IsRequired();

            this.Property(t => t.valorVendaLiquida)
               .HasPrecision(9, 3);

            this.Property(t => t.nsu)
                .HasMaxLength(255);

            this.Property(t => t.cdAutorizador)
                .HasMaxLength(255);

            this.Property(t => t.loteImportacao)
                .IsRequired()
                .HasMaxLength(35);

            this.Property(t => t.codTituloERP)
                .HasMaxLength(255);

            this.Property(t => t.codVendaERP)
                .HasMaxLength(255);

            this.Property(t => t.codResumoVenda)
                .HasMaxLength(255);

            this.Property(t => t.nrCartao)
                .HasMaxLength(20);

            this.Property(t => t.cdBandeira)
                .IsRequired();

            this.Property(t => t.cdSacado)
                .HasMaxLength(10)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            // Table & Column Mappings
            this.ToTable("Recebimento", "pos");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.idBandeira).HasColumnName("idBandeira");
            this.Property(t => t.cnpj).HasColumnName("cnpj");
            this.Property(t => t.nsu).HasColumnName("nsu");
            this.Property(t => t.cdAutorizador).HasColumnName("cdAutorizador");
            this.Property(t => t.dtaVenda).HasColumnName("dtaVenda");
            this.Property(t => t.valorVendaBruta).HasColumnName("valorVendaBruta");
            this.Property(t => t.valorVendaLiquida).HasColumnName("valorVendaLiquida");
            this.Property(t => t.loteImportacao).HasColumnName("loteImportacao");
            this.Property(t => t.dtaRecebimento).HasColumnName("dtaRecebimento");
            this.Property(t => t.idLogicoTerminal).HasColumnName("idLogicoTerminal");
            this.Property(t => t.codTituloERP).HasColumnName("codTituloERP");
            this.Property(t => t.codVendaERP).HasColumnName("codVendaERP");
            this.Property(t => t.codResumoVenda).HasColumnName("codResumoVenda");
            this.Property(t => t.numParcelaTotal).HasColumnName("numParcelaTotal");
            this.Property(t => t.cdBandeira).HasColumnName("cdBandeira");
            this.Property(t => t.idResumoVenda).HasColumnName("idResumoVenda");
            this.Property(t => t.nrCartao).HasColumnName("nrCartao");
            this.Property(t => t.idRecebimentoVenda).HasColumnName("idRecebimentoVenda");
            this.Property(t => t.cdSacado).HasColumnName("cdSacado");

            // Relationships
            this.HasRequired(t => t.empresa)
                .WithMany(t => t.Recebimentoes)
                .HasForeignKey(d => d.cnpj);
            this.HasRequired(t => t.BandeiraPos)
                .WithMany(t => t.Recebimentoes)
                .HasForeignKey(d => d.idBandeira);
            this.HasRequired(t => t.TerminalLogico)
                .WithMany(t => t.Recebimentoes)
                .HasForeignKey(d => d.idLogicoTerminal);
            this.HasRequired(t => t.tbBandeira)
                .WithMany(t => t.Recebimentos)
                .HasForeignKey(d => d.cdBandeira);
            this.HasOptional(t => t.tbResumoVenda)
                .WithMany(t => t.Recebimentos)
                .HasForeignKey(d => d.idResumoVenda);
            this.HasOptional(t => t.tbRecebimentoVenda)
                .WithMany(t => t.Recebimentos)
                .HasForeignKey(d => d.idRecebimentoVenda);
        }
    }
}
