using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbExtratoMap : EntityTypeConfiguration<tbExtrato>
    {
        public tbExtratoMap()
        {
            // Primary Key
            this.HasKey(t => t.idExtrato);

            // Properties
            this.Property(t => t.nrDocumento)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("tbExtrato", "card");
            this.Property(t => t.idExtrato).HasColumnName("idExtrato");
            this.Property(t => t.cdContaCorrente).HasColumnName("cdContaCorrente");
            this.Property(t => t.nrDocumento).HasColumnName("nrDocumento");
            this.Property(t => t.dtExtrato).HasColumnName("dtExtrato");
            this.Property(t => t.vlMovimento).HasColumnName("vlMovimento");
        }
    }
}