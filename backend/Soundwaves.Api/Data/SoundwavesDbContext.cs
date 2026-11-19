
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
    public DbSet<Playlist> Playlists => Set<Playlist>();
    public DbSet<PlaylistTrack> PlaylistTracks => Set<PlaylistTrack>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>().ToTable("artists");
        modelBuilder.Entity<Album>().ToTable("albums");
        modelBuilder.Entity<Track>().ToTable("tracks");
        modelBuilder.Entity<Playlist>().ToTable("playlists");

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

        // Playlist mapping
        modelBuilder.Entity<Playlist>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Playlist>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Playlist>()
            .Property(p => p.UserId)
            .HasColumnName("user_id");

        modelBuilder.Entity<Playlist>()
            .Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        modelBuilder.Entity<Playlist>()
            .Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        modelBuilder.Entity<Playlist>()
            .Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Playlist>()
            .Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .ValueGeneratedOnAddOrUpdate();

        modelBuilder.Entity<Playlist>()
            .HasIndex(p => p.UserId)
            .HasDatabaseName("idx_playlists_user_id");


        // Playlist-track relationship mapping
        modelBuilder.Entity<PlaylistTrack>()
            .ToTable("playlist_tracks");

        modelBuilder.Entity<PlaylistTrack>()
            .HasKey(pt => new
            {
                pt.PlaylistId,
                pt.TrackId,
                pt.Position
            });

        modelBuilder.Entity<PlaylistTrack>()
            .Property(pt => pt.PlaylistId)
            .HasColumnName("playlist_id");

        modelBuilder.Entity<PlaylistTrack>()
            .Property(pt => pt.TrackId)
            .HasColumnName("track_id");

        modelBuilder.Entity<PlaylistTrack>()
            .Property(pt => pt.Position)
            .HasColumnName("position");

        modelBuilder.Entity<PlaylistTrack>()
            .Property(pt => pt.AddedAt)
            .HasColumnName("added_at")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<PlaylistTrack>()
            .HasOne<Playlist>()
            .WithMany()
            .HasForeignKey(pt => pt.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlaylistTrack>()
            .HasOne<Track>()
            .WithMany()
            .HasForeignKey(pt => pt.TrackId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
