using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Server.Models;

namespace Server.Data
{
    public partial class ServerContext : DbContext
    {
        public ServerContext()
        {
        }

        public ServerContext(DbContextOptions<ServerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Localization> Localizations { get; set; } = null!;
        public virtual DbSet<Purchase> Purchases { get; set; } = null!;
        public virtual DbSet<Session> Sessions { get; set; } = null!;
        public virtual DbSet<Show> Shows { get; set; } = null!;
        public virtual DbSet<Theater> Theaters { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;Database=Servidor_Teatro;User Id=SA;Password=Sqlcomp123**;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.Property(e => e.DatePurchase).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.IdSessionNavigation)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.IdSession)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Purchase__id_ses__34C8D9D1");

                entity.HasOne(d => d.IdUsersNavigation)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.IdUsers)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Purchase__id_use__35BCFE0A");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasOne(d => d.IdShowNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.IdShow)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Session__id_show__2D27B809");
            });

            modelBuilder.Entity<Show>(entity =>
            {
                entity.HasOne(d => d.IdTheaterNavigation)
                    .WithMany(p => p.Shows)
                    .HasForeignKey(d => d.IdTheater)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Show__id_theater__2A4B4B5E");
            });

            modelBuilder.Entity<Theater>(entity =>
            {
                entity.HasOne(d => d.IdLocalizationNavigation)
                    .WithMany(p => p.Theaters)
                    .HasForeignKey(d => d.IdLocalization)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Theater__id_loca__267ABA7A");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(d => d.IdLocalizationNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdLocalization)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Users__id_locali__300424B4");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
