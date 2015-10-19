using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbRecebimentoResumoManualMap : EntityTypeConfiguration<tbRecebimentoResumoManual>
    {
        public tbRecebimentoResumoManualMap()
        {
            // Primary Key
            this.HasKey(t => t.idRecebimentoResumoManual);

            // Properties
            this.Property(t => t.cdTerminalLogico)
                .HasMaxLength(20);

            this.Property(t => t.dtVenda)
                .IsRequired();

            this.Property(t => t.vlVenda)
                .IsRequired();                        

            // Table & Column Mappings
            this.ToTable("tbRecebimentoResumoManual", "card");
            this.Property(t => t.idRecebimentoResumoManual).HasColumnName("idRecebimentoResumoManual");
            this.Property(t => t.cdTerminalLogico).HasColumnName("cdTerminalLogico");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.dtVenda).HasColumnName("dtVenda");
            this.Property(t => t.vlVenda).HasColumnName("vlVenda");
            this.Property(t => t.qtTracacao).HasColumnName("qtTracacao");            
            this.Property(t => t.cdBandeira).HasColumnName("cdBandeira");

            // Relationships
            this.HasOptional(t => t.tbAdquirente)
                .WithMany(t => t.tbRecebimentoResumoManuals)
                .HasForeignKey(t => t.cdAdquirente);
            this.HasOptional(t => t.tbBandeira)
                .WithMany(t => t.tbRecebimentoResumoManuals)
                .HasForeignKey(t => t.cdBandeira);
            this.HasOptional(t => t.tbTerminalLogico)
                .WithMany(t => t.tbRecebimentoResumoManuals)
                .HasForeignKey(t => t.cdTerminalLogico);
        }
    }
}

