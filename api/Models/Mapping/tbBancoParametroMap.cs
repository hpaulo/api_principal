using api.Models.Object;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models.Mapping
{
    public class tbBancoParametroMap : EntityTypeConfiguration<tbBancoParametro>
    {
        public tbBancoParametroMap()
        {
            // Primary Key
            this.HasKey(t => new { t.cdBanco, t.dsMemo, t.cdGrupo });

            // Properties
            this.Property(t => t.dsMemo)
                .IsRequired();

            this.Property(t => t.cdBanco)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.dsTipo)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.nrCnpj)
                .IsFixedLength()
                .HasMaxLength(14);

            this.Property(t => t.dsTipoCartao)
                .HasMaxLength(10);

            this.Property(t => t.flVisivel)
                .IsRequired();

            this.Property(t => t.flAntecipacao)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("tbBancoParametro", "card");
            this.Property(t => t.cdBanco).HasColumnName("cdBanco");
            this.Property(t => t.dsMemo).HasColumnName("dsMemo");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.dsTipo).HasColumnName("dsTipo");
            this.Property(t => t.flVisivel).HasColumnName("flVisivel");
            this.Property(t => t.nrCnpj).HasColumnName("nrCnpj");
            this.Property(t => t.dsTipoCartao).HasColumnName("dsTipoCartao");
            this.Property(t => t.cdBandeira).HasColumnName("cdBandeira");
            this.Property(t => t.flAntecipacao).HasColumnName("flAntecipacao");
            this.Property(t => t.cdGrupo).HasColumnName("cdGrupo");

            // Relationships
            this.HasOptional(t => t.tbAdquirente)
                .WithMany(t => t.tbBancoParametros)
                .HasForeignKey(d => d.cdAdquirente);
            this.HasOptional(t => t.empresa)
                .WithMany(t => t.tbBancoParametros)
                .HasForeignKey(d => d.nrCnpj);
            this.HasOptional(t => t.tbBandeira)
                .WithMany(t => t.tbBancoParametros)
                .HasForeignKey(d => d.cdBandeira);
            this.HasRequired(t => t.grupo_empresa)
                .WithMany(t => t.tbBancoParametros)
                .HasForeignKey(d => d.cdGrupo);
        }
    }
}
