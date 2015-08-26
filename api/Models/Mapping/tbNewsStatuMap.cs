using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbNewsStatuMap : EntityTypeConfiguration<tbNewsStatu>
    {
        public tbNewsStatuMap()
        {
            // Primary Key
            this.HasKey(t => new { t.idNews, t.id_users });

            // Properties
            this.Property(t => t.idNews)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.id_users)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("tbNewsStatus", "admin");
            this.Property(t => t.idNews).HasColumnName("idNews");
            this.Property(t => t.id_users).HasColumnName("id_users");
            this.Property(t => t.flRecebido).HasColumnName("flRecebido");
            this.Property(t => t.flLido).HasColumnName("flLido");

            // Relationships
            this.HasRequired(t => t.tbNew)
                .WithMany(t => t.tbNewsStatus)
                .HasForeignKey(d => d.idNews);
            this.HasRequired(t => t.webpages_Users)
                .WithMany(t => t.tbNewsStatus)
                .HasForeignKey(d => d.id_users);

        }
    }
}