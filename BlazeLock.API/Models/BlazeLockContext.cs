using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BlazeLock.API.Models;

public partial class BlazeLockContext : DbContext
{
    public BlazeLockContext()
    {
    }

    public BlazeLockContext(DbContextOptions<BlazeLockContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Coffre> Coffres { get; set; }

    public virtual DbSet<Dossier> Dossiers { get; set; }

    public virtual DbSet<Entree> Entrees { get; set; }

    public virtual DbSet<HistoriqueEntree> HistoriqueEntrees { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Partage> Partages { get; set; }

    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=BlazeLock;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coffre>(entity =>
        {
            entity.HasKey(e => e.IdCoffre).HasName("PK__Coffre__47508D12F46D3B31");

            entity.ToTable("Coffre");

            entity.Property(e => e.IdCoffre)
                .HasMaxLength(50)
                .HasColumnName("id_coffre");
            entity.Property(e => e.HashMasterkey)
                .HasMaxLength(50)
                .HasColumnName("hash_masterkey");
            entity.Property(e => e.IdUtilisateur)
                .HasMaxLength(50)
                .HasColumnName("id_utilisateur");
            entity.Property(e => e.Libelle)
                .HasMaxLength(50)
                .HasColumnName("libelle");
            entity.Property(e => e.Salt)
                .HasMaxLength(50)
                .HasColumnName("salt");

            entity.HasOne(d => d.IdUtilisateurNavigation).WithMany(p => p.Coffres)
                .HasForeignKey(d => d.IdUtilisateur)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Coffre__id_utili__398D8EEE");

            entity.HasMany(d => d.IdDossiers).WithMany(p => p.IdCoffres)
                .UsingEntity<Dictionary<string, object>>(
                    "Organiser",
                    r => r.HasOne<Dossier>().WithMany()
                        .HasForeignKey("IdDossier")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Organiser__Id_do__4BAC3F29"),
                    l => l.HasOne<Coffre>().WithMany()
                        .HasForeignKey("IdCoffre")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Organiser__id_co__4AB81AF0"),
                    j =>
                    {
                        j.HasKey("IdCoffre", "IdDossier").HasName("PK__Organise__B318C125E234F68B");
                        j.ToTable("Organiser");
                        j.IndexerProperty<string>("IdCoffre")
                            .HasMaxLength(50)
                            .HasColumnName("id_coffre");
                        j.IndexerProperty<string>("IdDossier")
                            .HasMaxLength(50)
                            .HasColumnName("Id_dossier");
                    });
        });

        modelBuilder.Entity<Dossier>(entity =>
        {
            entity.HasKey(e => e.IdDossier).HasName("PK__Dossier__4484C37EEDA5F640");

            entity.ToTable("Dossier");

            entity.Property(e => e.IdDossier)
                .HasMaxLength(50)
                .HasColumnName("Id_dossier");
            entity.Property(e => e.IdDossier1)
                .HasMaxLength(50)
                .HasColumnName("Id_dossier_1");
            entity.Property(e => e.Libelle)
                .HasMaxLength(50)
                .HasColumnName("libelle");

            entity.HasOne(d => d.IdDossier1Navigation).WithMany(p => p.InverseIdDossier1Navigation)
                .HasForeignKey(d => d.IdDossier1)
                .HasConstraintName("FK__Dossier__Id_doss__3C69FB99");

            entity.HasMany(d => d.IdEntrees).WithMany(p => p.IdDossiers)
                .UsingEntity<Dictionary<string, object>>(
                    "Stocker",
                    r => r.HasOne<Entree>().WithMany()
                        .HasForeignKey("IdEntree")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Stocker__id_entr__4F7CD00D"),
                    l => l.HasOne<Dossier>().WithMany()
                        .HasForeignKey("IdDossier")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Stocker__Id_doss__4E88ABD4"),
                    j =>
                    {
                        j.HasKey("IdDossier", "IdEntree").HasName("PK__Stocker__7F0E28E12110E1D5");
                        j.ToTable("Stocker");
                        j.IndexerProperty<string>("IdDossier")
                            .HasMaxLength(50)
                            .HasColumnName("Id_dossier");
                        j.IndexerProperty<string>("IdEntree")
                            .HasMaxLength(50)
                            .HasColumnName("id_entree");
                    });
        });

        modelBuilder.Entity<Entree>(entity =>
        {
            entity.HasKey(e => e.IdEntree).HasName("PK__Entree__B8AEB9F2EB302244");

            entity.ToTable("Entree");

            entity.Property(e => e.IdEntree)
                .HasMaxLength(50)
                .HasColumnName("id_entree");
            entity.Property(e => e.DateCreation)
                .HasColumnType("datetime")
                .HasColumnName("date_creation");
        });

        modelBuilder.Entity<HistoriqueEntree>(entity =>
        {
            entity.HasKey(e => e.IdHistorique).HasName("PK__Historiq__33F46ECB49A6FF51");

            entity.ToTable("Historique_entree");

            entity.Property(e => e.IdHistorique)
                .HasMaxLength(50)
                .HasColumnName("id_historique");
            entity.Property(e => e.Commentaire)
                .HasMaxLength(50)
                .HasColumnName("commentaire");
            entity.Property(e => e.CommentaireTag)
                .HasMaxLength(50)
                .HasColumnName("commentaire_tag");
            entity.Property(e => e.CommentaireVi)
                .HasMaxLength(50)
                .HasColumnName("commentaire_vi");
            entity.Property(e => e.DateUpdate)
                .HasMaxLength(50)
                .HasColumnName("date_update");
            entity.Property(e => e.IdEntree)
                .HasMaxLength(50)
                .HasColumnName("id_entree");
            entity.Property(e => e.Libelle)
                .HasMaxLength(50)
                .HasColumnName("libelle");
            entity.Property(e => e.LibelleTag)
                .HasMaxLength(50)
                .HasColumnName("libelle_tag");
            entity.Property(e => e.LibelleVi)
                .HasMaxLength(50)
                .HasColumnName("libelle_vi");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.PasswordTag)
                .HasMaxLength(50)
                .HasColumnName("password_tag");
            entity.Property(e => e.PasswordVi)
                .HasMaxLength(50)
                .HasColumnName("password_vi");
            entity.Property(e => e.Url)
                .HasMaxLength(50)
                .HasColumnName("url");
            entity.Property(e => e.UrlTag)
                .HasMaxLength(50)
                .HasColumnName("url_tag");
            entity.Property(e => e.UrlVi)
                .HasMaxLength(50)
                .HasColumnName("url_vi");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
            entity.Property(e => e.UsernameTag)
                .HasMaxLength(50)
                .HasColumnName("username_tag");
            entity.Property(e => e.UsernameVi)
                .HasMaxLength(50)
                .HasColumnName("username_vi");

            entity.HasOne(d => d.IdEntreeNavigation).WithMany(p => p.HistoriqueEntrees)
                .HasForeignKey(d => d.IdEntree)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historiqu__id_en__412EB0B6");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("PK__Log__6CC851FE0CAB883A");

            entity.ToTable("Log");

            entity.Property(e => e.IdLog)
                .HasMaxLength(50)
                .HasColumnName("id_log");
            entity.Property(e => e.IdCoffre)
                .HasMaxLength(50)
                .HasColumnName("id_coffre");
            entity.Property(e => e.Texte)
                .HasMaxLength(50)
                .HasColumnName("texte");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp_");

            entity.HasOne(d => d.IdCoffreNavigation).WithMany(p => p.Logs)
                .HasForeignKey(d => d.IdCoffre)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Log__id_coffre__440B1D61");
        });

        modelBuilder.Entity<Partage>(entity =>
        {
            entity.HasKey(e => new { e.IdUtilisateur, e.IdCoffre }).HasName("PK__Partage__2E3AAD6978ABB493");

            entity.ToTable("Partage");

            entity.Property(e => e.IdUtilisateur)
                .HasMaxLength(50)
                .HasColumnName("id_utilisateur");
            entity.Property(e => e.IdCoffre)
                .HasMaxLength(50)
                .HasColumnName("id_coffre");
            entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");

            entity.HasOne(d => d.IdCoffreNavigation).WithMany(p => p.Partages)
                .HasForeignKey(d => d.IdCoffre)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Partage__id_coff__47DBAE45");

            entity.HasOne(d => d.IdUtilisateurNavigation).WithMany(p => p.Partages)
                .HasForeignKey(d => d.IdUtilisateur)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Partage__id_util__46E78A0C");
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.IdUtilisateur).HasName("PK__Utilisat__1A4FA5B84005D655");

            entity.ToTable("Utilisateur");

            entity.Property(e => e.IdUtilisateur)
                .HasMaxLength(50)
                .HasColumnName("id_utilisateur");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
