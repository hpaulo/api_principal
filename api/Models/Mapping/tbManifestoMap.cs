using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbManifestoMap : EntityTypeConfiguration<tbManifesto>
    {
        public tbManifestoMap()
        {
            // Primary Key
            this.HasKey(t => t.idManifesto);

            // Properties
            this.Property(t => t.idManifesto)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.nrChave)
                .IsRequired()
                .HasMaxLength(44);

            this.Property(t => t.nrCNPJ)
                .IsRequired()
                .HasMaxLength(14);

            this.Property(t => t.nrEmitenteCNPJCPF)
                .IsRequired()
                .HasMaxLength(14);

            this.Property(t => t.nmEmitente)
                .IsRequired()
                .HasMaxLength(60);

            this.Property(t => t.nrEmitenteIE)
                .IsRequired()
                .HasMaxLength(14);

            this.Property(t => t.tpOperacao)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.nrNSU)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.cdSituacaoNFe)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.cdSituacaoManifesto)
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.dsSituacaoManifesto)
                .HasMaxLength(60);

            this.Property(t => t.nrProtocoloManifesto)
                .HasMaxLength(15);

            this.Property(t => t.nrProtocoloDownload)
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("tbManifesto", "tax");
            this.Property(t => t.idManifesto).HasColumnName("idManifesto");
            this.Property(t => t.nrChave).HasColumnName("nrChave");
            this.Property(t => t.cdGrupo).HasColumnName("cdGrupo");
            this.Property(t => t.nrCNPJ).HasColumnName("nrCNPJ");
            this.Property(t => t.nrEmitenteCNPJCPF).HasColumnName("nrEmitenteCNPJCPF");
            this.Property(t => t.nmEmitente).HasColumnName("nmEmitente");
            this.Property(t => t.nrEmitenteIE).HasColumnName("nrEmitenteIE");
            this.Property(t => t.dtEmissao).HasColumnName("dtEmissao");
            this.Property(t => t.tpOperacao).HasColumnName("tpOperacao");
            this.Property(t => t.vlNFe).HasColumnName("vlNFe");
            this.Property(t => t.dtRecebimento).HasColumnName("dtRecebimento");
            this.Property(t => t.nrNSU).HasColumnName("nrNSU");
            this.Property(t => t.cdSituacaoNFe).HasColumnName("cdSituacaoNFe");
            this.Property(t => t.cdSituacaoManifesto).HasColumnName("cdSituacaoManifesto");
            this.Property(t => t.dsSituacaoManifesto).HasColumnName("dsSituacaoManifesto");
            this.Property(t => t.nrProtocoloManifesto).HasColumnName("nrProtocoloManifesto");
            this.Property(t => t.xmlNFe).HasColumnName("xmlNFe");
            this.Property(t => t.nrProtocoloDownload).HasColumnName("nrProtocoloDownload");
        }
    }
}