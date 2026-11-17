
import { afterEach, describe, expect, it, vi } from "vitest";
import {
  cleanup,
  fireEvent,
  render,
  screen,
} from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import PlaylistDetails from "./PlaylistDetails";

afterEach(() => {
  cleanup();
});

describe("PlaylistDetails", () => {
  it("displays playlist details and tracks", () => {
    const playlist = {
      id: 1,
      name: "My Favorites",
      tracks: [
        { id: 1, title: "Sample Track" },
      ],
    };

    render(<PlaylistDetails playlist={playlist} />);

    expect(screen.getByText("My Favorites")).toBeInTheDocument();
    expect(screen.getByText("Sample Track")).toBeInTheDocument();
  });

  it("displays a message when the playlist is empty", () => {
    const playlist = {
      id: 2,
      name: "Chill Mix",
      tracks: [],
    };

    render(<PlaylistDetails playlist={playlist} />);

    expect(
      screen.getByText("No tracks in this playlist.")
    ).toBeInTheDocument();
  });

  it("calls onBack when Back to Playlists is clicked", () => {
    const playlist = {
      id: 1,
      name: "My Favorites",
      tracks: [],
    };

    const mockOnBack = vi.fn();

    render(
      <PlaylistDetails
        playlist={playlist}
        onBack={mockOnBack}
      />
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Back to Playlists" })
    );

    expect(mockOnBack).toHaveBeenCalledTimes(1);
  });

  it("calls onSelectTrack when a track is clicked", () => {
    const track = {
      id: 1,
      title: "Sample Track",
    };

    const playlist = {
      id: 1,
      name: "My Favorites",
      tracks: [track],
    };

    const mockOnSelectTrack = vi.fn();

    render(
      <PlaylistDetails
        playlist={playlist}
        onSelectTrack={mockOnSelectTrack}
      />
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Sample Track" })
    );

    expect(mockOnSelectTrack).toHaveBeenCalledWith(track);
  });

  it("displays a loading message", () => {
    render(<PlaylistDetails isLoading={true} />);

    expect(
      screen.getByText("Loading playlist...")
    ).toBeInTheDocument();

    expect(
      screen.getByRole("status")
    ).toBeInTheDocument();
  });

  it("displays an error message", () => {
    render(
      <PlaylistDetails error="Unable to load playlist." />
    );

    expect(
      screen.getByText("Unable to load playlist.")
    ).toBeInTheDocument();

    expect(
      screen.getByRole("alert")
    ).toBeInTheDocument();
  });
});
