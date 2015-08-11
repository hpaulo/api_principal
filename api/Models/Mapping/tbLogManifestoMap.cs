using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbLogManifestoMap : EntityTypeConfiguration<tbLogManifesto>
    {
        public tbLogManifestoMap()
        {
            // Primary Key
            this.HasKey(t => t.idLog);

            // Properties
            this.Property(t => t.dsComando)
                .IsRequired();

            this.Property(t => t.cdRetorno)
                .HasMaxLength(20);

            this.Property(t => t.dsRetorno)
                .HasMaxLength(50);

            this.Property(t => t.dsMetodo)
                .HasMaxLength(30);

            this.Property(t => t.tpLog)
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("tbLogManifesto", "admin");
            this.Property(t => t.idLog).HasColumnName("idLog");
            this.Property(t => t.dtLog).HasColumnName("dtLog");
            this.Property(t => t.dsComando).HasColumnName("dsComando");
            this.Property(t => t.cdRetorno).HasColumnName("cdRetorno");
            this.Property(t => t.dsRetorno).HasColumnName("dsRetorno");
            this.Property(t => t.dsMetodo).HasColumnName("dsMetodo");
            this.Property(t => t.tpLog).HasColumnName("tpLog");
        }
    }
}