
import { afterEach, describe, expect, it, vi } from "vitest";
import {
  cleanup,
  fireEvent,
  render,
  screen,
  waitFor,
} from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import PlaylistDetails from "./PlaylistDetails";

afterEach(() => {
  cleanup();
});

describe("PlaylistDetails", () => {
  // Existing playlist tests

  it("displays playlist details and tracks", () => {
    const playlist = {
      id: 1,
      name: "My Favorites",
      tracks: [{ id: 1, title: "Sample Track" }],
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

    expect(screen.getByRole("status")).toBeInTheDocument();
  });

  it("displays an error message", () => {
    render(
      <PlaylistDetails error="Unable to load playlist." />
    );

    expect(
      screen.getByText("Unable to load playlist.")
    ).toBeInTheDocument();

    expect(screen.getByRole("alert")).toBeInTheDocument();
  });

  // SCRUM-171: Remove Track tests

  const playlist = {
    id: 10,
    name: "Study Playlist",
    tracks: [
      { id: 3, title: "Demo Tone A" },
      { id: 4, title: "Demo Tone B" },
    ],
  };

  it("shows Remove Track buttons for playlist tracks", () => {
    render(<PlaylistDetails playlist={playlist} />);

    expect(
      screen.getAllByRole("button", { name: "Remove Track" })
    ).toHaveLength(2);
  });

  it("shows confirmation when Remove Track is clicked", () => {
    render(<PlaylistDetails playlist={playlist} />);

    fireEvent.click(
      screen.getAllByRole("button", { name: "Remove Track" })[0]
    );

    expect(
      screen.getByText("Remove Demo Tone A from this playlist?")
    ).toBeInTheDocument();

    expect(
      screen.getByRole("button", { name: "Confirm Remove" })
    ).toBeInTheDocument();
  });

  it("cancels removal without calling the callback", () => {
    const mockRemove = vi.fn();

    render(
      <PlaylistDetails
        playlist={playlist}
        onRemoveTrack={mockRemove}
      />
    );

    fireEvent.click(
      screen.getAllByRole("button", { name: "Remove Track" })[0]
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Cancel" })
    );

    expect(mockRemove).not.toHaveBeenCalled();

    expect(
      screen.queryByRole("button", { name: "Confirm Remove" })
    ).not.toBeInTheDocument();

    expect(screen.getByText("Demo Tone A")).toBeInTheDocument();
  });

  it("calls onRemoveTrack with playlist and track IDs", async () => {
    const mockRemove = vi.fn().mockResolvedValue(undefined);

    render(
      <PlaylistDetails
        playlist={playlist}
        onRemoveTrack={mockRemove}
      />
    );

    fireEvent.click(
      screen.getAllByRole("button", { name: "Remove Track" })[0]
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Confirm Remove" })
    );

    await waitFor(() => {
      expect(mockRemove).toHaveBeenCalledWith(10, 3);
    });

    expect(mockRemove).toHaveBeenCalledTimes(1);

    expect(
      await screen.findByText(
        "Remove request completed. Refresh playlist details to confirm."
      )
    ).toBeInTheDocument();
  });

  it("shows an error if removal fails", async () => {
    const mockRemove = vi.fn().mockRejectedValue(
      new Error("Backend unavailable")
    );

    render(
      <PlaylistDetails
        playlist={playlist}
        onRemoveTrack={mockRemove}
      />
    );

    fireEvent.click(
      screen.getAllByRole("button", { name: "Remove Track" })[0]
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Confirm Remove" })
    );

    expect(
      await screen.findByRole("alert")
    ).toHaveTextContent(
      "Unable to remove track from playlist. Please try again."
    );
  });

  it("shows an error if the remove callback is not connected", async () => {
    render(<PlaylistDetails playlist={playlist} />);

    fireEvent.click(
      screen.getAllByRole("button", { name: "Remove Track" })[0]
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Confirm Remove" })
    );

    expect(
      await screen.findByRole("alert")
    ).toHaveTextContent(
      "Unable to remove track from playlist. Please try again."
    );
  });

  it("prevents duplicate removal while waiting", async () => {
    let finishRemoval;

    const mockRemove = vi.fn().mockImplementation(
      () => new Promise((resolve) => {
        finishRemoval = resolve;
      })
    );

    render(
      <PlaylistDetails
        playlist={playlist}
        onRemoveTrack={mockRemove}
      />
    );

    fireEvent.click(
      screen.getAllByRole("button", { name: "Remove Track" })[0]
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Confirm Remove" })
    );

    expect(
      screen.getByRole("button", { name: "Removing..." })
    ).toBeDisabled();

    expect(mockRemove).toHaveBeenCalledTimes(1);

    finishRemoval();

    await waitFor(() => {
      expect(
        screen.getByText(
          "Remove request completed. Refresh playlist details to confirm."
        )
      ).toBeInTheDocument();
    });
  });
});
