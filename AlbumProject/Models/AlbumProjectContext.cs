using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AlbumProject.Models;

public partial class AlbumProjectContext : DbContext
{
    public AlbumProjectContext()
    {
    }

    public AlbumProjectContext(DbContextOptions<AlbumProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-OMVS92A\\MSSQLSERVER01; Initial Catalog=AlbumProject; Integrated Security=True; Pooling=False; Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasOne(d => d.File).WithMany(p => p.Albums).HasConstraintName("FK_Albums_File");
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasOne(d => d.Album).WithMany(p => p.Songs).HasConstraintName("FK_Albums_Songs");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
