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
                .HasMaxLength(41)
                .IsRequired();

            //this.Property(t => t.cdAdquirente)
            //    .IsRequired();

            this.Property(t => t.dsBandeira)
                .HasMaxLength(50);

            this.Property(t => t.vlVenda)
                .IsRequired();

            this.Property(t => t.dtVenda)
                .IsRequired();

            this.Property(t => t.qtParcelas)
                .IsRequired();

            this.Property(t => t.cdERP)
                .HasMaxLength(40)
                .IsRequired();

            //this.Property(t => t.cdERPPagamento)
            //    .HasMaxLength(15);

            this.Property(t => t.cdSacado)
                .HasMaxLength(10);

            this.Property(t => t.flInsert)
                .IsRequired();

            this.Property(t => t.cdAdquirente)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

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
            this.Property(t => t.cdSacado).HasColumnName("cdSacado");
            this.Property(t => t.dtAjuste).HasColumnName("dtAjuste");
            this.Property(t => t.flInsert).HasColumnName("flInsert");
            //this.Property(t => t.cdERPPagamento).HasColumnName("cdERPPagamento");

            // Relationships
            //this.HasOptional(t => t.tbAdquirente)
            //    .WithMany(t => t.tbRecebimentoVendas)
            //    .HasForeignKey(d => d.cdAdquirente);
            this.HasRequired(t => t.empresa)
                .WithMany(t => t.tbRecebimentoVendas)
                .HasForeignKey(d => d.nrCNPJ);
        }
    }
}