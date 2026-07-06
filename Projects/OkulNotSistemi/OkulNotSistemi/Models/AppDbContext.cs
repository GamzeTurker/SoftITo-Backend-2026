using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OkulNotSistemi.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dersler> Derslers { get; set; }

    public virtual DbSet<Notlar> Notlars { get; set; }

    public virtual DbSet<Ogrenciler> Ogrencilers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OkulNotSistemi;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dersler>(entity =>
        {
            entity.HasKey(e => e.DersId).HasName("PK__Derslers__E8B3DE11AB73BA3A");

            entity.Property(e => e.DersAd).HasMaxLength(100);
        });

        modelBuilder.Entity<Notlar>(entity =>
        {
            entity.HasKey(e => e.NotId).HasName("PK__Notlars__4FB2008A83A3B71F");

            entity.Property(e => e.Final).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Vize).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Ders).WithMany(p => p.Notlars)
                .HasForeignKey(d => d.DersId)
                .HasConstraintName("FK__Notlars__DersId__619B8048");

            entity.HasOne(d => d.Ogrenci).WithMany(p => p.Notlars)
                .HasForeignKey(d => d.OgrenciId)
                .HasConstraintName("FK__Notlars__Ogrenci__60A75C0F");
        });

        modelBuilder.Entity<Ogrenciler>(entity =>
        {
            entity.HasKey(e => e.OgrenciId).HasName("PK__Ogrencil__E497E6B4609791AF");

            entity.Property(e => e.Numara).HasMaxLength(20);
            entity.Property(e => e.OgrenciAd).HasMaxLength(50);
            entity.Property(e => e.OgrenciSoyad).HasMaxLength(50);
            entity.Property(e => e.Sinif).HasMaxLength(10);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
