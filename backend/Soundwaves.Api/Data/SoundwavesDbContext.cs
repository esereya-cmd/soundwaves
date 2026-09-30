using Microsoft.EntityFrameworkCore;
using Soundwaves.Api.Models;

namespace Soundwaves.Api.Data;

public class SoundwavesDbContext : DbContext
{
    public SoundwavesDbContext(DbContextOptions<SoundwavesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Track> Tracks => Set<Track>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>().ToTable("artists");
        modelBuilder.Entity<Album>().ToTable("albums");
        modelBuilder.Entity<Track>().ToTable("tracks");

        modelBuilder.Entity<Artist>()
            .Property(a => a.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<Album>()
            .Property(a => a.ArtistId)
            .HasColumnName("artist_id");

        modelBuilder.Entity<Album>()
            .Property(a => a.ReleaseDate)
            .HasColumnName("release_date");

        modelBuilder.Entity<Album>()
            .Property(a => a.ArtworkPath)
            .HasColumnName("artwork_path");

        modelBuilder.Entity<Album>()
            .Property(a => a.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<Track>()
            .Property(t => t.AlbumId)
            .HasColumnName("album_id");

        modelBuilder.Entity<Track>()
            .Property(t => t.ArtistId)
            .HasColumnName("artist_id");

        modelBuilder.Entity<Track>()
            .Property(t => t.DurationMs)
            .HasColumnName("duration_ms");

        modelBuilder.Entity<Track>()
            .Property(t => t.TrackNumber)
            .HasColumnName("track_number");

        modelBuilder.Entity<Track>()
            .Property(t => t.FilePath)
            .HasColumnName("file_path");

        modelBuilder.Entity<Track>()
            .Property(t => t.MimeType)
            .HasColumnName("mime_type");

        modelBuilder.Entity<Track>()
            .Property(t => t.FileSizeBytes)
            .HasColumnName("file_size_bytes");

        modelBuilder.Entity<Track>()
            .Property(t => t.CreatedAt)
            .HasColumnName("created_at");
    }
}