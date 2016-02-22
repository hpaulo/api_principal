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

            this.Property(t => t.vlOperacao)
                .HasPrecision(9, 2)
                //.IsRequired();
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            this.Property(t => t.vlLiquido)
                .HasPrecision(9, 2)
                //.IsRequired();
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            this.Property(t => t.dtAntecipacaoBancaria)
                .IsRequired();

            this.Property(t => t.cdAdquirente)
                .IsRequired();

            this.Property(t => t.cdContaCorrente)
                .IsRequired();

            this.Property(t => t.txJuros)
                .HasPrecision(6, 3)
                .IsRequired();

            this.Property(t => t.txIOF)
                .HasPrecision(5, 4)
                .IsRequired();

            this.Property(t => t.txIOFAdicional)
                .HasPrecision(5, 4)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("tbAntecipacaoBancaria", "card");
            this.Property(t => t.idAntecipacaoBancaria).HasColumnName("idAntecipacaoBancaria");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.cdContaCorrente).HasColumnName("cdContaCorrente");
            this.Property(t => t.dtAntecipacaoBancaria).HasColumnName("dtAntecipacaoBancaria");
            this.Property(t => t.vlOperacao).HasColumnName("vlOperacao");
            this.Property(t => t.vlLiquido).HasColumnName("vlLiquido");
            this.Property(t => t.txJuros).HasColumnName("txJuros");
            this.Property(t => t.txIOF).HasColumnName("txIOF");
            this.Property(t => t.txIOFAdicional).HasColumnName("txIOFAdicional");

            // Relationships
            this.HasRequired(t => t.tbAdquirente)
                .WithMany(t => t.tbAntecipacaoBancarias)
                .HasForeignKey(d => d.cdAdquirente);
            this.HasRequired(t => t.tbContaCorrente)
                .WithMany(t => t.tbAntecipacaoBancarias)
                .HasForeignKey(d => d.cdContaCorrente);
        }
    }
}