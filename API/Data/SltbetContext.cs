using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public partial class SltbetContext : DbContext
{
    
    public SltbetContext()
    {
    }

    public SltbetContext(DbContextOptions<SltbetContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Luta> Lutas { get; set; }

    public virtual DbSet<Lutador> Lutadores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Luta>(entity =>
        {
            entity.HasKey(e => e.LutaId).HasName("PRIMARY");

            entity.ToTable("lutas");

            entity.HasIndex(e => e.Lutador1, "Lutador1");

            entity.HasIndex(e => e.Lutador2, "fk_table1_Lutadores1_idx");

            entity.Property(e => e.LutaId).ValueGeneratedNever();
            entity.Property(e => e.Lutador1).HasMaxLength(50);
            entity.Property(e => e.Lutador2).HasMaxLength(50);

            entity.HasOne(d => d.Lutador1Navigation).WithMany(p => p.LutaLutador1Navigations)
                .HasForeignKey(d => d.Lutador1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Lutador1");

            entity.HasOne(d => d.Lutador2Navigation).WithMany(p => p.LutaLutador2Navigations)
                .HasForeignKey(d => d.Lutador2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Lutador2");
        });

        modelBuilder.Entity<Lutador>(entity =>
        {
            entity.HasKey(e => e.Nome).HasName("PRIMARY");

            entity.ToTable("lutadores");

            entity.HasIndex(e => e.Nome, "Nome_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Pasta, "Pasta_UNIQUE").IsUnique();

            entity.Property(e => e.Nome).HasMaxLength(50);
            entity.Property(e => e.Imagem).HasMaxLength(200);
            entity.Property(e => e.Pasta).HasMaxLength(100);
            entity.Property(e => e.Tier)
                .HasDefaultValueSql("'F'")
                .HasColumnType("enum('F','D','C','B','A','S','SS','SSS','UBER')");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
