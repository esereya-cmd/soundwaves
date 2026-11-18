
import { afterEach, describe, expect, it, vi } from "vitest";
import {
  cleanup,
  fireEvent,
  render,
  screen,
} from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import PlaylistList from "./PlaylistList";

afterEach(() => {
  cleanup();
});

describe("PlaylistList", () => {
  it("displays available playlists", () => {
    const playlists = [
      { id: 1, name: "My Favorites", tracks: [] },
      { id: 2, name: "Chill Mix", tracks: [] },
    ];

    render(<PlaylistList playlists={playlists} />);

    expect(screen.getByText("My Favorites")).toBeInTheDocument();
    expect(screen.getByText("Chill Mix")).toBeInTheDocument();
  });

  it("displays a message when no playlists exist", () => {
    render(<PlaylistList playlists={[]} />);

    expect(screen.getByText("No playlists yet.")).toBeInTheDocument();
  });

  it("calls onSelectPlaylist when a playlist is selected", () => {
    const playlists = [
      { id: 1, name: "My Favorites", tracks: [] },
    ];

    const mockOnSelectPlaylist = vi.fn();

    render(
      <PlaylistList
        playlists={playlists}
        onSelectPlaylist={mockOnSelectPlaylist}
      />
    );

    fireEvent.click(
      screen.getByRole("button", { name: "View Playlist" })
    );

    expect(mockOnSelectPlaylist).toHaveBeenCalledWith(playlists[0]);
  });

  it("displays a loading message", () => {
    render(<PlaylistList isLoading={true} />);

    expect(
      screen.getByText("Loading playlists...")
    ).toBeInTheDocument();

    expect(screen.getByRole("status")).toBeInTheDocument();
  });

  it("displays an error message", () => {
    render(
      <PlaylistList error="Unable to load playlists." />
    );

    expect(
      screen.getByText("Unable to load playlists.")
    ).toBeInTheDocument();

    expect(screen.getByRole("alert")).toBeInTheDocument();
  });
});

