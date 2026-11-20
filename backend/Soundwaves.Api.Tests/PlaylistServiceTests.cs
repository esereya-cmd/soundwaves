
using Microsoft.EntityFrameworkCore;
using Soundwaves.Api.Data;
using Soundwaves.Api.DTOs;
using Soundwaves.Api.Models;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Tests;

public class PlaylistServiceTests
{
    private static SoundwavesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SoundwavesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SoundwavesDbContext(options);
    }

    [Fact]
    public async Task CreatePlaylistAsync_CreatesPlaylist()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new CreatePlaylistRequest
        {
            Name = "My Favorites",
            Description = "Favorite songs"
        };

        var result = await service.CreatePlaylistAsync(1001, request);

        Assert.Equal("My Favorites", result.Name);
        Assert.Equal("Favorite songs", result.Description);
        Assert.Empty(result.Tracks);

        var saved = await dbContext.Playlists.SingleAsync();

        Assert.Equal((ulong)1001, saved.UserId);
        Assert.Equal("My Favorites", saved.Name);
    }

    [Fact]
    public async Task CreatePlaylistAsync_TrimsPlaylistName()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new CreatePlaylistRequest
        {
            Name = "  Study Music  "
        };

        var result = await service.CreatePlaylistAsync(1001, request);

        Assert.Equal("Study Music", result.Name);

        var saved = await dbContext.Playlists.SingleAsync();

        Assert.Equal("Study Music", saved.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreatePlaylistAsync_RejectsEmptyNames(
        string name)
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new CreatePlaylistRequest
        {
            Name = name
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreatePlaylistAsync(1001, request));

        Assert.Empty(dbContext.Playlists);
    }

    [Fact]
    public async Task CreatePlaylistAsync_RejectsLongName()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new CreatePlaylistRequest
        {
            Name = new string('A', 256)
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreatePlaylistAsync(1001, request));

        Assert.Empty(dbContext.Playlists);
    }

    [Fact]
    public async Task CreatePlaylistAsync_RejectsLongDescription()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new CreatePlaylistRequest
        {
            Name = "Test Playlist",
            Description = new string('A', 1001)
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreatePlaylistAsync(1001, request));

        Assert.Empty(dbContext.Playlists);
    }

    [Fact]
    public async Task CreatePlaylistAsync_RejectsInvalidUserId()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new CreatePlaylistRequest
        {
            Name = "Test Playlist"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreatePlaylistAsync(0, request));

        Assert.Empty(dbContext.Playlists);
    }

    [Fact]
    public async Task GetPlaylistsAsync_ReturnsOnlyCurrentUsersPlaylists()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.AddRange(
            new Playlist
            {
                Id = 1,
                UserId = 1001,
                Name = "My Playlist"
            },
            new Playlist
            {
                Id = 2,
                UserId = 2002,
                Name = "Another User Playlist"
            }
        );

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var results = (await service.GetPlaylistsAsync(1001)).ToList();

        Assert.Single(results);
        Assert.Equal("My Playlist", results[0].Name);
        Assert.Equal((ulong)1, results[0].Id);
    }

    [Fact]
    public async Task GetPlaylistsAsync_ReturnsEmptyWhenNoPlaylistsExist()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var results = await service.GetPlaylistsAsync(1001);

        Assert.Empty(results);
    }

    [Fact]
    public async Task GetPlaylistsAsync_RejectsInvalidUserId()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetPlaylistsAsync(0));
    }

    [Fact]
    public async Task GetPlaylistsAsync_ReturnsPlaylistsInIdOrder()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.AddRange(
            new Playlist
            {
                Id = 3,
                UserId = 1001,
                Name = "Third"
            },
            new Playlist
            {
                Id = 1,
                UserId = 1001,
                Name = "First"
            },
            new Playlist
            {
                Id = 2,
                UserId = 1001,
                Name = "Second"
            }
        );

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var results = (await service.GetPlaylistsAsync(1001)).ToList();

        Assert.Equal(
            new ulong[] { 1, 2, 3 },
            results.Select(p => p.Id));
    }

    [Fact]
    public async Task GetPlaylistByIdAsync_ReturnsPlaylist_WhenOwnedByUser()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "My Favorites",
            Description = "Favorite songs"
        });

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var result = await service.GetPlaylistByIdAsync(1001, 1);

        Assert.NotNull(result);
        Assert.Equal((ulong)1, result.Id);
        Assert.Equal("My Favorites", result.Name);
        Assert.Equal("Favorite songs", result.Description);
        Assert.Empty(result.Tracks);
    }

    [Fact]
    public async Task GetPlaylistByIdAsync_ReturnsNull_WhenPlaylistDoesNotExist()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var result = await service.GetPlaylistByIdAsync(1001, 999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetPlaylistByIdAsync_ReturnsNull_WhenOwnedByAnotherUser()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.Add(new Playlist
        {
            Id = 2,
            UserId = 2002,
            Name = "Private Playlist"
        });

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var result = await service.GetPlaylistByIdAsync(1001, 2);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetPlaylistByIdAsync_ReturnsTracksInPositionOrder()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Ordered Playlist"
        });

        dbContext.Tracks.AddRange(
            new Track
            {
                Id = 10,
                Title = "First Song",
                FilePath = "/first.mp3"
            },
            new Track
            {
                Id = 20,
                Title = "Second Song",
                FilePath = "/second.mp3"
            }
        );

        dbContext.PlaylistTracks.AddRange(
            new PlaylistTrack
            {
                PlaylistId = 1,
                TrackId = 20,
                Position = 2
            },
            new PlaylistTrack
            {
                PlaylistId = 1,
                TrackId = 10,
                Position = 1
            }
        );

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var result = await service.GetPlaylistByIdAsync(1001, 1);

        Assert.NotNull(result);
        Assert.Equal(2, result.Tracks.Count);
        Assert.Equal((ulong)10, result.Tracks[0].Id);
        Assert.Equal((uint)1, result.Tracks[0].Position);
        Assert.Equal((ulong)20, result.Tracks[1].Id);
        Assert.Equal((uint)2, result.Tracks[1].Position);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1001, 0)]
    public async Task GetPlaylistByIdAsync_RejectsInvalidIds(
        ulong userId,
        ulong playlistId)
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetPlaylistByIdAsync(userId, playlistId));
    }

    [Fact]
    public async Task UpdatePlaylistAsync_UpdatesPlaylist_WhenOwnedByUser()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Original Playlist",
            Description = "Original description"
        });

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var request = new UpdatePlaylistRequest
        {
            Name = "Updated Playlist",
            Description = "Updated description"
        };

        var result = await service.UpdatePlaylistAsync(
            1001, 1, request);

        Assert.NotNull(result);
        Assert.Equal("Updated Playlist", result.Name);
        Assert.Equal("Updated description", result.Description);

        var saved = await dbContext.Playlists.SingleAsync();

        Assert.Equal((ulong)1001, saved.UserId);
        Assert.Equal("Updated Playlist", saved.Name);
        Assert.Equal("Updated description", saved.Description);
    }

    [Fact]
    public async Task UpdatePlaylistAsync_TrimsPlaylistName()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Original"
        });

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var request = new UpdatePlaylistRequest
        {
            Name = "  Updated Favorites  ",
            Description = null
        };

        var result = await service.UpdatePlaylistAsync(
            1001, 1, request);

        Assert.NotNull(result);
        Assert.Equal("Updated Favorites", result.Name);
        Assert.Null(result.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdatePlaylistAsync_RejectsEmptyNames(
        string name)
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Original"
        });

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var request = new UpdatePlaylistRequest
        {
            Name = name,
            Description = "Test"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdatePlaylistAsync(1001, 1, request));

        var saved = await dbContext.Playlists.SingleAsync();

        Assert.Equal("Original", saved.Name);
    }

    [Fact]
    public async Task UpdatePlaylistAsync_RejectsLongName()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Original"
        });

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var request = new UpdatePlaylistRequest
        {
            Name = new string('A', 256)
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdatePlaylistAsync(1001, 1, request));

        Assert.Equal(
            "Original",
            (await dbContext.Playlists.SingleAsync()).Name);
    }

    [Fact]
    public async Task UpdatePlaylistAsync_RejectsLongDescription()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Original"
        });

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var request = new UpdatePlaylistRequest
        {
            Name = "Updated",
            Description = new string('A', 1001)
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdatePlaylistAsync(1001, 1, request));

        Assert.Equal(
            "Original",
            (await dbContext.Playlists.SingleAsync()).Name);
    }

    [Fact]
    public async Task UpdatePlaylistAsync_ReturnsNull_WhenPlaylistDoesNotExist()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new UpdatePlaylistRequest
        {
            Name = "Updated",
            Description = "Test"
        };

        var result = await service.UpdatePlaylistAsync(
            1001, 999, request);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePlaylistAsync_ReturnsNull_WhenOwnedByAnotherUser()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.Add(new Playlist
        {
            Id = 2,
            UserId = 2002,
            Name = "Private Playlist",
            Description = "Private description"
        });

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var request = new UpdatePlaylistRequest
        {
            Name = "Unauthorized Update",
            Description = "Should not change"
        };

        var result = await service.UpdatePlaylistAsync(
            1001, 2, request);

        Assert.Null(result);

        var saved = await dbContext.Playlists.SingleAsync();

        Assert.Equal("Private Playlist", saved.Name);
        Assert.Equal("Private description", saved.Description);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1001, 0)]
    public async Task UpdatePlaylistAsync_RejectsInvalidIds(
        ulong userId,
        ulong playlistId)
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new UpdatePlaylistRequest
        {
            Name = "Updated"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdatePlaylistAsync(
                userId, playlistId, request));
    }

    [Fact]
    public async Task DeletePlaylistAsync_DeletesOwnedPlaylist()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Delete Me"
        });

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var result = await service.DeletePlaylistAsync(1001, 1);

        Assert.True(result);
        Assert.Empty(await dbContext.Playlists.ToListAsync());
    }

    [Fact]
    public async Task DeletePlaylistAsync_ReturnsFalse_WhenPlaylistDoesNotExist()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var result = await service.DeletePlaylistAsync(1001, 999);

        Assert.False(result);
    }

    [Fact]
    public async Task DeletePlaylistAsync_ReturnsFalse_WhenOwnedByAnotherUser()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.Add(new Playlist
        {
            Id = 2,
            UserId = 2002,
            Name = "Private Playlist"
        });

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var result = await service.DeletePlaylistAsync(1001, 2);

        Assert.False(result);

        var saved = await dbContext.Playlists.SingleAsync();

        Assert.Equal((ulong)2002, saved.UserId);
        Assert.Equal("Private Playlist", saved.Name);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1001, 0)]
    public async Task DeletePlaylistAsync_RejectsInvalidIds(
        ulong userId,
        ulong playlistId)
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.DeletePlaylistAsync(userId, playlistId));
    }

    [Fact]
    public async Task DeletePlaylistAsync_PreservesOtherPlaylists()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.AddRange(
            new Playlist
            {
                Id = 1,
                UserId = 1001,
                Name = "Delete Me"
            },
            new Playlist
            {
                Id = 2,
                UserId = 1001,
                Name = "Keep Me"
            }
        );

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var result = await service.DeletePlaylistAsync(1001, 1);

        Assert.True(result);

        var remaining = await dbContext.Playlists.ToListAsync();

        Assert.Single(remaining);
        Assert.Equal((ulong)2, remaining[0].Id);
        Assert.Equal("Keep Me", remaining[0].Name);
    }

    [Fact]
    public async Task DeletePlaylistAsync_PreservesCatalogTracks()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Temporary Playlist"
        });

        dbContext.Tracks.Add(new Track
        {
            Id = 10,
            Title = "Demo Track",
            FilePath = "/demo.mp3"
        });

        dbContext.PlaylistTracks.Add(new PlaylistTrack
        {
            PlaylistId = 1,
            TrackId = 10,
            Position = 1
        });

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var result = await service.DeletePlaylistAsync(1001, 1);

        Assert.True(result);
        Assert.Empty(await dbContext.Playlists.ToListAsync());

        // Deleting a playlist must not delete catalog tracks.
        var track = await dbContext.Tracks.SingleAsync();

        Assert.Equal((ulong)10, track.Id);
        Assert.Equal("Demo Track", track.Title);
    }

    [Fact]
    public async Task AddTrackToPlaylistAsync_AddsExistingTrack()
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Test Playlist"
        });

        db.Tracks.Add(new Track
        {
            Id = 10,
            Title = "Test Song",
            FilePath = "/test.mp3"
        });

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        var result = await service.AddTrackToPlaylistAsync(
            1001, 1, 10);

        Assert.NotNull(result);
        Assert.Equal((ulong)10, result.Id);
        Assert.Equal("Test Song", result.Title);
        Assert.Equal((uint)1, result.Position);

        var saved = await db.PlaylistTracks.SingleAsync();

        Assert.Equal((ulong)1, saved.PlaylistId);
        Assert.Equal((ulong)10, saved.TrackId);
        Assert.Equal((uint)1, saved.Position);
    }

    [Fact]
    public async Task AddTrackToPlaylistAsync_AssignsNextPosition()
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Test Playlist"
        });

        db.Tracks.AddRange(
            new Track
            {
                Id = 10,
                Title = "First Song",
                FilePath = "/first.mp3"
            },
            new Track
            {
                Id = 20,
                Title = "Second Song",
                FilePath = "/second.mp3"
            }
        );

        db.PlaylistTracks.Add(new PlaylistTrack
        {
            PlaylistId = 1,
            TrackId = 10,
            Position = 1
        });

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        var result = await service.AddTrackToPlaylistAsync(
            1001, 1, 20);

        Assert.NotNull(result);
        Assert.Equal((uint)2, result.Position);
        Assert.Equal(2, await db.PlaylistTracks.CountAsync());
    }

    [Fact]
    public async Task AddTrackToPlaylistAsync_RejectsDuplicateTrack()
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Test Playlist"
        });

        db.Tracks.Add(new Track
        {
            Id = 10,
            Title = "Test Song",
            FilePath = "/test.mp3"
        });

        db.PlaylistTracks.Add(new PlaylistTrack
        {
            PlaylistId = 1,
            TrackId = 10,
            Position = 1
        });

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AddTrackToPlaylistAsync(1001, 1, 10));

        Assert.Single(await db.PlaylistTracks.ToListAsync());
    }

    [Fact]
    public async Task AddTrackToPlaylistAsync_ReturnsNull_WhenTrackMissing()
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Test Playlist"
        });

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        var result = await service.AddTrackToPlaylistAsync(
            1001, 1, 999);

        Assert.Null(result);
        Assert.Empty(await db.PlaylistTracks.ToListAsync());
    }

    [Fact]
    public async Task AddTrackToPlaylistAsync_ReturnsNull_WhenPlaylistMissing()
    {
        using var db = CreateDbContext();
        var service = new PlaylistService(db);

        var result = await service.AddTrackToPlaylistAsync(
            1001, 999, 10);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddTrackToPlaylistAsync_PreventsOtherUserAccess()
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 2,
            UserId = 2002,
            Name = "Private Playlist"
        });

        db.Tracks.Add(new Track
        {
            Id = 10,
            Title = "Test Song",
            FilePath = "/test.mp3"
        });

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        var result = await service.AddTrackToPlaylistAsync(
            1001, 2, 10);

        Assert.Null(result);
        Assert.Empty(await db.PlaylistTracks.ToListAsync());
    }

    [Theory]
    [InlineData(0, 1, 10)]
    [InlineData(1001, 0, 10)]
    [InlineData(1001, 1, 0)]
    public async Task AddTrackToPlaylistAsync_RejectsInvalidIds(
        ulong userId,
        ulong playlistId,
        ulong trackId)
    {
        using var db = CreateDbContext();
        var service = new PlaylistService(db);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.AddTrackToPlaylistAsync(
                userId, playlistId, trackId));
    }

    [Fact]
    public async Task RemoveTrackFromPlaylistAsync_RemovesTrackAndRenumbers()
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Test Playlist"
        });

        db.PlaylistTracks.AddRange(
            new PlaylistTrack { PlaylistId = 1, TrackId = 10, Position = 1 },
            new PlaylistTrack { PlaylistId = 1, TrackId = 20, Position = 2 },
            new PlaylistTrack { PlaylistId = 1, TrackId = 30, Position = 3 }
        );

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        var result = await service.RemoveTrackFromPlaylistAsync(
            1001, 1, 20);

        Assert.True(result);

        var tracks = await db.PlaylistTracks
            .OrderBy(pt => pt.Position)
            .ToListAsync();

        Assert.Equal(2, tracks.Count);
        Assert.Equal((ulong)10, tracks[0].TrackId);
        Assert.Equal((uint)1, tracks[0].Position);
        Assert.Equal((ulong)30, tracks[1].TrackId);
        Assert.Equal((uint)2, tracks[1].Position);
    }

    [Fact]
    public async Task RemoveTrackFromPlaylistAsync_ReturnsFalse_WhenMissing()
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Test Playlist"
        });

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        Assert.False(
            await service.RemoveTrackFromPlaylistAsync(1001, 1, 999));
    }

    [Fact]
    public async Task RemoveTrackFromPlaylistAsync_PreventsOtherUserAccess()
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 2,
            UserId = 2002,
            Name = "Private Playlist"
        });

        db.PlaylistTracks.Add(new PlaylistTrack
        {
            PlaylistId = 2,
            TrackId = 10,
            Position = 1
        });

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        Assert.False(
            await service.RemoveTrackFromPlaylistAsync(1001, 2, 10));

        Assert.Single(await db.PlaylistTracks.ToListAsync());
    }

    [Fact]
    public async Task ReorderPlaylistTracksAsync_UpdatesPositions()
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Test Playlist"
        });

        db.PlaylistTracks.AddRange(
            new PlaylistTrack { PlaylistId = 1, TrackId = 10, Position = 1 },
            new PlaylistTrack { PlaylistId = 1, TrackId = 20, Position = 2 },
            new PlaylistTrack { PlaylistId = 1, TrackId = 30, Position = 3 }
        );

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        var result = await service.ReorderPlaylistTracksAsync(
            1001, 1, new ulong[] { 30, 10, 20 });

        Assert.True(result);

        var tracks = await db.PlaylistTracks
            .OrderBy(pt => pt.Position)
            .ToListAsync();

        Assert.Equal(
            new ulong[] { 30, 10, 20 },
            tracks.Select(pt => pt.TrackId));

        Assert.Equal(
            new uint[] { 1, 2, 3 },
            tracks.Select(pt => pt.Position));
    }

    [Theory]
    [InlineData(new ulong[] { 10, 10 })]
    [InlineData(new ulong[] { 10 })]
    [InlineData(new ulong[] { 10, 999 })]
    public async Task ReorderPlaylistTracksAsync_RejectsInvalidTrackLists(
        ulong[] trackIds)
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Test Playlist"
        });

        db.PlaylistTracks.AddRange(
            new PlaylistTrack { PlaylistId = 1, TrackId = 10, Position = 1 },
            new PlaylistTrack { PlaylistId = 1, TrackId = 20, Position = 2 }
        );

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.ReorderPlaylistTracksAsync(
                1001, 1, trackIds));

        var tracks = await db.PlaylistTracks
            .OrderBy(pt => pt.Position)
            .ToListAsync();

        Assert.Equal(
            new ulong[] { 10, 20 },
            tracks.Select(pt => pt.TrackId));
    }

    [Fact]
    public async Task ReorderPlaylistTracksAsync_PreventsOtherUserAccess()
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 2,
            UserId = 2002,
            Name = "Private Playlist"
        });

        db.PlaylistTracks.Add(new PlaylistTrack
        {
            PlaylistId = 2,
            TrackId = 10,
            Position = 1
        });

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        var result = await service.ReorderPlaylistTracksAsync(
            1001, 2, new ulong[] { 10 });

        Assert.False(result);
    }

    [Fact]
    public async Task ReorderPlaylistTracksAsync_AllowsEmptyPlaylist()
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Empty Playlist"
        });

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        var result = await service.ReorderPlaylistTracksAsync(
            1001, 1, Array.Empty<ulong>());

        Assert.True(result);
        Assert.Empty(await db.PlaylistTracks.ToListAsync());
    }

    [Fact]
    public async Task CreatePlaylistAsync_RejectsNullRequest()
    {
        using var db = CreateDbContext();
        var service = new PlaylistService(db);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.CreatePlaylistAsync(1001, null!));

        Assert.Empty(await db.Playlists.ToListAsync());
    }

    [Fact]
    public async Task UpdatePlaylistAsync_RejectsNullRequest()
    {
        using var db = CreateDbContext();
        var service = new PlaylistService(db);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.UpdatePlaylistAsync(1001, 1, null!));

        Assert.Empty(await db.Playlists.ToListAsync());
    }

    [Theory]
    [InlineData(0, 1, 10)]
    [InlineData(1001, 0, 10)]
    [InlineData(1001, 1, 0)]
    public async Task RemoveTrackFromPlaylistAsync_RejectsInvalidIds(
        ulong userId,
        ulong playlistId,
        ulong trackId)
    {
        using var db = CreateDbContext();
        var service = new PlaylistService(db);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.RemoveTrackFromPlaylistAsync(
                userId, playlistId, trackId));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1001, 0)]
    public async Task ReorderPlaylistTracksAsync_RejectsInvalidIds(
        ulong userId,
        ulong playlistId)
    {
        using var db = CreateDbContext();
        var service = new PlaylistService(db);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.ReorderPlaylistTracksAsync(
                userId, playlistId, Array.Empty<ulong>()));
    }

    [Fact]
    public async Task ReorderPlaylistTracksAsync_RejectsNullTrackList()
    {
        using var db = CreateDbContext();
        var service = new PlaylistService(db);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.ReorderPlaylistTracksAsync(
                1001, 1, null!));
    }

    [Fact]
    public async Task CreatePlaylistAsync_AcceptsMaximumLengths()
    {
        using var db = CreateDbContext();
        var service = new PlaylistService(db);

        var request = new CreatePlaylistRequest
        {
            Name = new string('N', 255),
            Description = new string('D', 1000)
        };

        var result = await service.CreatePlaylistAsync(
            1001, request);

        Assert.Equal(255, result.Name.Length);
        Assert.Equal(1000, result.Description!.Length);

        var saved = await db.Playlists.SingleAsync();

        Assert.Equal(255, saved.Name.Length);
        Assert.Equal(1000, saved.Description!.Length);
    }

    [Fact]
    public async Task UpdatePlaylistAsync_AcceptsMaximumLengths()
    {
        using var db = CreateDbContext();

        db.Playlists.Add(new Playlist
        {
            Id = 1,
            UserId = 1001,
            Name = "Original"
        });

        await db.SaveChangesAsync();

        var service = new PlaylistService(db);

        var request = new UpdatePlaylistRequest
        {
            Name = new string('N', 255),
            Description = new string('D', 1000)
        };

        var result = await service.UpdatePlaylistAsync(
            1001, 1, request);

        Assert.NotNull(result);
        Assert.Equal(255, result.Name.Length);
        Assert.Equal(1000, result.Description!.Length);

        var saved = await db.Playlists.SingleAsync();

        Assert.Equal(255, saved.Name.Length);
        Assert.Equal(1000, saved.Description!.Length);
    }

}
