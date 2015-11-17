﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace api.Models.Mapping
{
    public class tbRecebimentoTituloMap: EntityTypeConfiguration<tbRecebimentoTitulo>
    {
        public tbRecebimentoTituloMap()
        {
            // Primary Key
            this.HasKey(t => t.idRecebimentoTitulo);

            // Properties
            this.Property(t => t.nrCNPJ)
                .IsFixedLength()
                .HasMaxLength(14)
                .IsRequired();

            this.Property(t => t.nrNSU)
                .HasMaxLength(30)
                .IsRequired();

            this.Property(t => t.cdAdquirente)
                .IsRequired();

            this.Property(t => t.dsBandeira)
                .HasMaxLength(50);

            this.Property(t => t.dtTitulo)
                .IsRequired();

            this.Property(t => t.vlParcela)
                .IsRequired();

            this.Property(t => t.nrParcela)
                .IsRequired();

            this.Property(t => t.cdERP)
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("tbRecebimentoTitulo", "card");
            this.Property(t => t.idRecebimentoTitulo).HasColumnName("idRecebimentoTitulo");
            this.Property(t => t.nrCNPJ).HasColumnName("nrCNPJ");
            this.Property(t => t.nrNSU).HasColumnName("nrNSU");
            this.Property(t => t.dtVenda).HasColumnName("dtVenda");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.dsBandeira).HasColumnName("dsBandeira");
            this.Property(t => t.vlVenda).HasColumnName("vlVenda");
            this.Property(t => t.qtParcelas).HasColumnName("qtParcelas");
            this.Property(t => t.dtTitulo).HasColumnName("dtTitulo");
            this.Property(t => t.vlParcela).HasColumnName("vlParcela");
            this.Property(t => t.nrParcela).HasColumnName("nrParcela");
            this.Property(t => t.cdERP).HasColumnName("cdERP");
            this.Property(t => t.dtBaixaERP).HasColumnName("dtBaixaERP");

            // Relationships
            this.HasRequired(t => t.tbAdquirente)
                .WithMany(t => t.tbRecebimentoTitulos)
                .HasForeignKey(d => d.cdAdquirente);
            this.HasRequired(t => t.empresa)
                .WithMany(t => t.tbRecebimentoTitulos)
                .HasForeignKey(d => d.nrCNPJ);
        }
    }
}