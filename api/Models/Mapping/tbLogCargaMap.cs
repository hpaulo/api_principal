using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbLogCargaMap : EntityTypeConfiguration<tbLogCarga>
    {
        public tbLogCargaMap()
        {
            // Primary Key
            this.HasKey(t => t.idLogCarga);

            // Properties
            this.Property(t => t.dtCompetencia)
                .IsRequired();

            this.Property(t => t.nrCNPJ)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(14);

            this.Property(t => t.cdAdquirente)
                .IsRequired();

            this.Property(t => t.flStatusVendasCredito)
                .IsRequired();

            this.Property(t => t.flStatusVendasDebito)
                .IsRequired();

            this.Property(t => t.flStatusPagosCredito)
                .IsRequired();

            this.Property(t => t.flStatusPagosDebito)
                .IsRequired();

            this.Property(t => t.flStatusPagosAntecipacao)
                .IsRequired();

            this.Property(t => t.flStatusReceber)
                .IsRequired();

            this.Property(t => t.vlPagosAntecipacao)
                .HasPrecision(9, 2)
                .IsRequired();

            this.Property(t => t.vlPagosCredito)
                .HasPrecision(9, 2)
                .IsRequired();

            this.Property(t => t.vlPagosDebito)
                .HasPrecision(9, 2)
                .IsRequired();

            this.Property(t => t.vlVendaCredito)
                .HasPrecision(9, 2)
                .IsRequired();

            this.Property(t => t.vlVendaDebito)
                .HasPrecision(9, 2)
                .IsRequired();


            // Table & Column Mappings
            this.ToTable("tbLogCarga", "card");
            this.Property(t => t.idLogCarga).HasColumnName("idLogCarga");
            this.Property(t => t.dtCompetencia).HasColumnName("dtCompetencia");
            this.Property(t => t.nrCNPJ).HasColumnName("nrCNPJ");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.flStatusVendasCredito).HasColumnName("flStatusVendasCredito");
            this.Property(t => t.flStatusVendasDebito).HasColumnName("flStatusVendasDebito");
            this.Property(t => t.flStatusPagosCredito).HasColumnName("flStatusPagosCredito");
            this.Property(t => t.flStatusPagosDebito).HasColumnName("flStatusPagosDebito");
            this.Property(t => t.flStatusPagosAntecipacao).HasColumnName("flStatusPagosAntecipacao");
            this.Property(t => t.flStatusReceber).HasColumnName("flStatusReceber");
            this.Property(t => t.vlPagosAntecipacao).HasColumnName("vlPagosAntecipacao");
            this.Property(t => t.vlPagosCredito).HasColumnName("vlPagosCredito");
            this.Property(t => t.vlPagosDebito).HasColumnName("vlPagosDebito");
            this.Property(t => t.vlVendaCredito).HasColumnName("vlVendaCredito");
            this.Property(t => t.vlVendaDebito).HasColumnName("vlVendaDebito");

            // Relationships
            this.HasRequired(t => t.tbAdquirente)
                .WithMany(t => t.tbLogCargas)
                .HasForeignKey(d => d.cdAdquirente);
            this.HasRequired(t => t.empresa)
                .WithMany(t => t.tbLogCargas)
                .HasForeignKey(d => d.nrCNPJ);

        }
    }
}