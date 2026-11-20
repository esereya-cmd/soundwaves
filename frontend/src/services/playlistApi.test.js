
import { afterEach, describe, expect, it, vi } from "vitest";

import {
  getPlaylists,
  getPlaylist,
  createPlaylist,
  updatePlaylist,
  deletePlaylist,
  addTrack,
  removeTrack,
} from "./playlistApi";

afterEach(() => {
  vi.unstubAllGlobals();
  vi.restoreAllMocks();
});

describe("Playlist API Service", () => {
  it("gets all playlists", async () => {
    const playlists = [{ id: 1, name: "My Favorites" }];

    const mockFetch = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => playlists,
    });

    vi.stubGlobal("fetch", mockFetch);

    const result = await getPlaylists();

    expect(mockFetch).toHaveBeenCalledWith("/api/playlists");
    expect(result).toEqual(playlists);
  });

  it("gets playlist details", async () => {
    const playlist = {
      id: 1,
      name: "My Favorites",
      tracks: [{ id: 3, title: "Demo Tone A" }],
    };

    const mockFetch = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => playlist,
    });

    vi.stubGlobal("fetch", mockFetch);

    const result = await getPlaylist(1);

    expect(mockFetch).toHaveBeenCalledWith("/api/playlists/1");
    expect(result).toEqual(playlist);
  });

  it("creates a playlist", async () => {
    const playlist = {
      id: 1,
      name: "Study Music",
      description: null,
      tracks: [],
    };

    const mockFetch = vi.fn().mockResolvedValue({
      ok: true,
      status: 201,
      json: async () => playlist,
    });

    vi.stubGlobal("fetch", mockFetch);

    const result = await createPlaylist("Study Music");

    expect(mockFetch).toHaveBeenCalledWith(
      "/api/playlists",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          name: "Study Music",
          description: null,
        }),
      }
    );

    expect(result).toEqual(playlist);
  });

  it("updates a playlist", async () => {
    const playlist = {
      id: 1,
      name: "New Favorites",
      description: null,
    };

    const mockFetch = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => playlist,
    });

    vi.stubGlobal("fetch", mockFetch);

    const result = await updatePlaylist(
      1,
      "New Favorites"
    );

    expect(mockFetch).toHaveBeenCalledWith(
      "/api/playlists/1",
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          name: "New Favorites",
          description: null,
        }),
      }
    );

    expect(result).toEqual(playlist);
  });

  it("deletes a playlist", async () => {
    const mockFetch = vi.fn().mockResolvedValue({
      ok: true,
      status: 204,
    });

    vi.stubGlobal("fetch", mockFetch);

    const result = await deletePlaylist(1);

    expect(mockFetch).toHaveBeenCalledWith(
      "/api/playlists/1",
      { method: "DELETE" }
    );

    expect(result).toBeNull();
  });

  it("adds a track to a playlist", async () => {
    const track = {
      id: 3,
      title: "Demo Tone A",
      position: 1,
    };

    const mockFetch = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => track,
    });

    vi.stubGlobal("fetch", mockFetch);

    const result = await addTrack(1, 3);

    expect(mockFetch).toHaveBeenCalledWith(
      "/api/playlists/1/tracks",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          trackId: 3,
        }),
      }
    );

    expect(result).toEqual(track);
  });

  it("removes a track from a playlist", async () => {
    const mockFetch = vi.fn().mockResolvedValue({
      ok: true,
      status: 204,
    });

    vi.stubGlobal("fetch", mockFetch);

    const result = await removeTrack(1, 3);

    expect(mockFetch).toHaveBeenCalledWith(
      "/api/playlists/1/tracks/3",
      { method: "DELETE" }
    );

    expect(result).toBeNull();
  });

  it("handles duplicate-track errors", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: false,
        status: 409,
      })
    );

    await expect(addTrack(1, 3)).rejects.toThrow(
      "This track is already in the playlist."
    );
  });

  it("handles missing playlists or tracks", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: false,
        status: 404,
      })
    );

    await expect(getPlaylist(999)).rejects.toThrow(
      "Playlist or track not found."
    );
  });

  it("handles unauthorized requests", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: false,
        status: 401,
      })
    );

    await expect(getPlaylists()).rejects.toThrow(
      "You are not authorized."
    );
  });

  it("handles invalid playlist information", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: false,
        status: 400,
      })
    );

    await expect(createPlaylist("")).rejects.toThrow(
      "Invalid playlist information."
    );
  });

  it("handles unexpected server errors", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: false,
        status: 500,
      })
    );

    await expect(getPlaylists()).rejects.toThrow(
      "Playlist request failed."
    );
  });
});
