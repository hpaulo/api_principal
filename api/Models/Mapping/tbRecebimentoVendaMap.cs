using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbRecebimentoVendaMap : EntityTypeConfiguration<tbRecebimentoVenda>
    {
        public tbRecebimentoVendaMap()
        {
            // Primary Key
            this.HasKey(t => t.idRecebimentoVenda);

            // Properties
            this.Property(t => t.nrCNPJ)
                .IsFixedLength()
                .HasMaxLength(14)
                .IsRequired();

            this.Property(t => t.nrNSU)
                .HasMaxLength(30)
                .IsRequired();

            this.Property(t => t.cdAdquirente)
                .IsRequired();

            this.Property(t => t.dsBandeira)
                .HasMaxLength(50);

            this.Property(t => t.vlVenda)
                .IsRequired();

            this.Property(t => t.dtVenda)
                .IsRequired();

            this.Property(t => t.qtParcelas)
                .IsRequired();

            this.Property(t => t.cdERP)
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("tbRecebimentoVenda", "card");
            this.Property(t => t.idRecebimentoVenda).HasColumnName("idRecebimentoVenda");
            this.Property(t => t.nrCNPJ).HasColumnName("nrCNPJ");
            this.Property(t => t.nrNSU).HasColumnName("nrNSU");
            this.Property(t => t.dtVenda).HasColumnName("dtVenda");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.dsBandeira).HasColumnName("dsBandeira");
            this.Property(t => t.vlVenda).HasColumnName("vlVenda");
            this.Property(t => t.qtParcelas).HasColumnName("qtParcelas");
            this.Property(t => t.cdERP).HasColumnName("cdERP");

            // Relationships
            this.HasRequired(t => t.tbAdquirente)
                .WithMany(t => t.tbRecebimentoVendas)
                .HasForeignKey(d => d.cdAdquirente);
            this.HasRequired(t => t.empresa)
                .WithMany(t => t.tbRecebimentoVendas)
                .HasForeignKey(d => d.nrCNPJ);
        }
    }
}