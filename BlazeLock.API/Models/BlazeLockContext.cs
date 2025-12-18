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
    {

        if (optionsBuilder.IsConfigured == false)
        {
           optionsBuilder.UseSqlServer("Server=localhost;Database=BlazeLock;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coffre>(entity =>
        {
            entity.HasKey(e => e.IdCoffre);

            entity.ToTable("Coffre");

            entity.Property(e => e.IdCoffre)
                .HasMaxLength(50)
                .HasColumnName("id_coffre");
            // Explicit SQL type for binary data
            entity.Property(e => e.HashMasterkey)
                .HasColumnName("hash_masterkey")
                .HasColumnType("varbinary(50)");
            entity.Property(e => e.IdUtilisateur)
                .HasMaxLength(50)
                .HasColumnName("id_utilisateur");
            entity.Property(e => e.Libelle)
                .HasMaxLength(50)
                .HasColumnName("libelle");
            entity.Property(e => e.Salt)
                .HasColumnName("salt")
                .HasColumnType("varbinary(50)");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Coffres)
                .HasForeignKey(d => d.IdUtilisateur)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Dossier>(entity =>
        {
            entity.HasKey(e => e.IdDossier);

            entity.ToTable("Dossier");

            entity.Property(e => e.IdDossier)
                .HasMaxLength(50)
                .HasColumnName("Id_dossier");
            entity.Property(e => e.Libelle)
                .HasMaxLength(50)
                .HasColumnName("libelle");
            entity.Property(e => e.IdCoffre)
                .HasColumnName("id_coffre");

            entity.HasOne(d => d.Coffre).WithMany(p => p.Dossiers)
                .HasForeignKey(d => d.IdCoffre)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Entree>(entity =>
        {
            entity.HasKey(e => e.IdEntree);

            entity.ToTable("Entree");

            entity.Property(e => e.IdEntree)
                .HasMaxLength(50)
                .HasColumnName("id_entree");
            entity.Property(e => e.DateCreation)
                .HasColumnType("datetime")
                .HasColumnName("date_creation");
            entity.Property(e => e.IdDossier)
                .HasMaxLength(50)
                .HasColumnName("id_dossier");

            entity.HasOne(d => d.Dossier).WithMany(p => p.Entrees)
                .HasForeignKey(d => d.IdDossier)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<HistoriqueEntree>(entity =>
        {
            entity.HasKey(e => e.IdHistorique);

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

            entity.HasOne(d => d.Entree).WithMany(p => p.HistoriqueEntrees)
                .HasForeignKey(d => d.IdEntree)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.IdLog);

            entity.ToTable("Log");

            entity.Property(e => e.IdLog)
                .HasMaxLength(50)
                .HasColumnName("id_log");
            entity.Property(e => e.IdCoffre)
                .HasMaxLength(50)
                .HasColumnName("id_coffre");
            entity.Property(e => e.Texte)
                .HasMaxLength(500)
                .HasColumnName("texte");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp_");

            entity.HasOne(d => d.Coffre).WithMany(p => p.Logs)
                .HasForeignKey(d => d.IdCoffre)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Logs)
                .HasForeignKey(d => d.IdUtilisateur)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Partage>(entity =>
        {
            entity.HasKey(e => new { e.IdUtilisateur, e.IdCoffre });

            entity.ToTable("Partage");

            entity.Property(e => e.IdUtilisateur)
                .HasMaxLength(50)
                .HasColumnName("id_utilisateur");
            entity.Property(e => e.IdCoffre)
                .HasMaxLength(50)
                .HasColumnName("id_coffre");
            entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");

            entity.HasOne(d => d.Coffre).WithMany(p => p.Partages)
                .HasForeignKey(d => d.IdCoffre)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Partages)
                .HasForeignKey(d => d.IdUtilisateur)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.IdUtilisateur);

            entity.ToTable("Utilisateur");

            entity.Property(e => e.IdUtilisateur)
                .HasMaxLength(50)
                .HasColumnName("id_utilisateur");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
