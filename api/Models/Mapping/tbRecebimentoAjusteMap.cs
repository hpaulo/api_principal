using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbRecebimentoAjusteMap : EntityTypeConfiguration<tbRecebimentoAjuste>
    {
        public tbRecebimentoAjusteMap()
        {
            // Primary Key
            this.HasKey(t => t.idRecebimentoAjuste);

            // Properties
            this.Property(t => t.dtAjuste)
                .IsRequired();

            this.Property(t => t.nrCNPJ)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(14);

            this.Property(t => t.cdBandeira)
                .IsRequired();

            this.Property(t => t.dsMotivo)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.vlAjuste)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("tbRecebimentoAjuste", "card");
            this.Property(t => t.idRecebimentoAjuste).HasColumnName("idRecebimentoAjuste");
            this.Property(t => t.dtAjuste).HasColumnName("dtAjuste");
            this.Property(t => t.nrCNPJ).HasColumnName("nrCNPJ");
            this.Property(t => t.cdBandeira).HasColumnName("cdBandeira");
            this.Property(t => t.dsMotivo).HasColumnName("dsMotivo");
            this.Property(t => t.vlAjuste).HasColumnName("vlAjuste");
            this.Property(t => t.idExtrato).HasColumnName("idExtrato");
            this.Property(t => t.idResumoVenda).HasColumnName("idResumoVenda");

            // Relationships
            this.HasRequired(t => t.tbBandeira)
                .WithMany(t => t.tbRecebimentoAjustes)
                .HasForeignKey(d => d.cdBandeira);
            this.HasRequired(t => t.empresa)
                .WithMany(t => t.tbRecebimentoAjustes)
                .HasForeignKey(d => d.nrCNPJ);
            this.HasOptional(t => t.tbExtrato)
                .WithMany(t => t.tbRecebimentoAjustes)
                .HasForeignKey(d => d.idExtrato);
            this.HasOptional(t => t.tbResumoVenda)
                .WithMany(t => t.tbRecebimentoAjustes)
                .HasForeignKey(d => d.idResumoVenda);
        }
    }
}

