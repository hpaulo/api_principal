using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbLogCargaDetalheMap : EntityTypeConfiguration<tbLogCargaDetalhe>
    {
        public tbLogCargaDetalheMap()
        {
            // Primary Key
            this.HasKey(t => t.idLogCargaDetalhe);

            // Properties
            this.Property(t => t.idLogCarga)
                .IsRequired();

            this.Property(t => t.dtExecucaoIni)
                .IsRequired();

            this.Property(t => t.flStatus)
                .IsRequired();

            this.Property(t => t.dsMensagem)
                .HasMaxLength(250);

            this.Property(t => t.dsModalidade)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("tbLogCargaDetalhe", "card");
            this.Property(t => t.idLogCargaDetalhe).HasColumnName("idLogCargaDetalhe");
            this.Property(t => t.idLogCarga).HasColumnName("idLogCarga");
            this.Property(t => t.dtExecucaoIni).HasColumnName("dtExecucaoIni");
            this.Property(t => t.dtExecucaoFim).HasColumnName("dtExecucaoFim");
            this.Property(t => t.flStatus).HasColumnName("flStatus");
            this.Property(t => t.dsMensagem).HasColumnName("dsMensagem");
            this.Property(t => t.dsModalidade).HasColumnName("dsModalidade");
            this.Property(t => t.qtTransacoes).HasColumnName("qtTransacoes");
            this.Property(t => t.vlTotalProcessado).HasColumnName("vlTotalProcessado");

            // Relationships
            this.HasRequired(t => t.tbLogCargas)
                .WithMany(t => t.tbLogCargaDetalhes)
                .HasForeignKey(d => d.idLogCarga);

        }
    }
}