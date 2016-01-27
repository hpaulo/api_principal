using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbResumoVendaMap : EntityTypeConfiguration<tbResumoVenda>
    {
        public tbResumoVendaMap()
        {
            // Primary Key
            this.HasKey(t => t.idResumoVenda);

            // Properties
            this.Property(t => t.cdResumoVenda)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.dtVenda)
                .IsRequired();

            this.Property(t => t.dtRecebimentoPrevisto)
                .IsRequired();

            this.Property(t => t.vlTotal)
                .IsRequired();

            this.Property(t => t.qtCVs)
                .IsRequired();

            this.Property(t => t.cdBandeira)
                .IsRequired();

            this.Property(t => t.nrCNPJ)
                .IsFixedLength()
                .HasMaxLength(14)
                .IsRequired();

            this.Property(t => t.nrLinha)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("tbResumoVenda", "card");
            this.Property(t => t.idResumoVenda).HasColumnName("idResumoVenda");
            this.Property(t => t.cdResumoVenda).HasColumnName("cdResumoVenda");
            this.Property(t => t.dtRecebimentoPrevisto).HasColumnName("dtRecebimentoPrevisto");
            this.Property(t => t.vlTotal).HasColumnName("vlTotal");
            this.Property(t => t.qtCVs).HasColumnName("qtCVs");
            this.Property(t => t.cdBandeira).HasColumnName("cdBandeira");
            this.Property(t => t.nrCNPJ).HasColumnName("nrCNPJ");
            this.Property(t => t.nrLinha).HasColumnName("nrLinha");

            // Relationships
            this.HasRequired(t => t.tbBandeira)
                .WithMany(t => t.tbResumoVendas)
                .HasForeignKey(d => d.cdBandeira);
            this.HasRequired(t => t.empresa)
                .WithMany(t => t.tbResumoVendas)
                .HasForeignKey(d => d.nrCNPJ);
        }
    }
}