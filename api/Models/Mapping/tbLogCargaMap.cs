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
            this.Property(t => t.nrCNPJ)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(14);

            // Table & Column Mappings
            this.ToTable("tbLogCarga", "card");
            this.Property(t => t.idLogCarga).HasColumnName("idLogCarga");
            this.Property(t => t.dtCompetencia).HasColumnName("dtCompetencia");
            this.Property(t => t.nrCNPJ).HasColumnName("nrCNPJ");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.flStatusVenda).HasColumnName("flStatusVenda");
            this.Property(t => t.flStatusPagos).HasColumnName("flStatusPagos");
            this.Property(t => t.flStatusReceber).HasColumnName("flStatusReceber");

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