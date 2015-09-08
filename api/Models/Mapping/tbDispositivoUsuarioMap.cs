using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbDispositivoUsuarioMap : EntityTypeConfiguration<tbDispositivoUsuario>
    {
        public tbDispositivoUsuarioMap()
        {
            // Primary Key
            this.HasKey(t => t.idDispositivoUsuario);

            // Properties
            this.Property(t => t.dsPlataforma)
                .HasMaxLength(50);

            this.Property(t => t.dsModelo)
                .HasMaxLength(50);

            this.Property(t => t.dsVersao)
                .HasMaxLength(50);

            this.Property(t => t.idIONIC)
                .HasMaxLength(50);

            this.Property(t => t.idUserIONIC)
                .HasMaxLength(50);

            this.Property(t => t.cdTokenValido)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tbDispositivoUsuario", "admin");
            this.Property(t => t.idDispositivoUsuario).HasColumnName("idDispositivoUsuario");
            this.Property(t => t.idUser).HasColumnName("idUser");
            this.Property(t => t.dsPlataforma).HasColumnName("dsPlataforma");
            this.Property(t => t.dsModelo).HasColumnName("dsModelo");
            this.Property(t => t.dsVersao).HasColumnName("dsVersao");
            this.Property(t => t.idIONIC).HasColumnName("idIONIC");
            this.Property(t => t.idUserIONIC).HasColumnName("idUserIONIC");
            this.Property(t => t.cdTokenValido).HasColumnName("cdTokenValido");
            this.Property(t => t.tmLargura).HasColumnName("tmLargura");
            this.Property(t => t.tmAltura).HasColumnName("tmAltura");
        }
    }
}