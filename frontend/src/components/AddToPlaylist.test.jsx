
import { afterEach, describe, expect, it, vi } from "vitest";
import {
  cleanup,
  fireEvent,
  render,
  screen,
  waitFor,
} from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import AddToPlaylist from "./AddToPlaylist";

afterEach(() => {
  cleanup();
});

describe("AddToPlaylist", () => {
  const track = {
    id: 1,
    title: "Sample Track",
  };

  const playlists = [
    { id: 1, name: "My Favorites" },
    { id: 2, name: "Chill Mix" },
  ];

  it("displays the selected track and playlists", () => {
    render(
      <AddToPlaylist
        track={track}
        playlists={playlists}
      />
    );

    expect(
      screen.getByText("Selected Track: Sample Track")
    ).toBeInTheDocument();

    expect(
      screen.getByRole("option", { name: "My Favorites" })
    ).toBeInTheDocument();

    expect(
      screen.getByRole("option", { name: "Chill Mix" })
    ).toBeInTheDocument();
  });

  it("shows an error when no playlist is selected", () => {
    render(
      <AddToPlaylist
        track={track}
        playlists={playlists}
      />
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Add Track" })
    );

    expect(
      screen.getByText("Please select a playlist.")
    ).toBeInTheDocument();
  });

  it("submits the selected playlist and track", async () => {
    const mockAddTrack = vi.fn().mockResolvedValue();

    render(
      <AddToPlaylist
        track={track}
        playlists={playlists}
        onAddTrack={mockAddTrack}
      />
    );

    fireEvent.change(
      screen.getByLabelText("Choose Playlist:"),
      { target: { value: "2" } }
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Add Track" })
    );

    await waitFor(() => {
      expect(mockAddTrack).toHaveBeenCalledWith(2, 1);
    });
  });

  it("prevents duplicate submission while adding a track", async () => {
    let finishRequest;

    const mockAddTrack = vi.fn(
      () =>
        new Promise((resolve) => {
          finishRequest = resolve;
        })
    );

    render(
      <AddToPlaylist
        track={track}
        playlists={playlists}
        onAddTrack={mockAddTrack}
      />
    );

    fireEvent.change(
      screen.getByLabelText("Choose Playlist:"),
      { target: { value: "1" } }
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Add Track" })
    );

    expect(
      screen.getByRole("button", { name: "Adding..." })
    ).toBeDisabled();

    expect(mockAddTrack).toHaveBeenCalledTimes(1);

    finishRequest();

    await waitFor(() => {
      expect(
        screen.getByRole("button", { name: "Add Track" })
      ).not.toBeDisabled();
    });
  });

  it("shows an error when adding the track fails", async () => {
    const mockAddTrack = vi.fn().mockRejectedValue(
      new Error("Request failed")
    );

    render(
      <AddToPlaylist
        track={track}
        playlists={playlists}
        onAddTrack={mockAddTrack}
      />
    );

    fireEvent.change(
      screen.getByLabelText("Choose Playlist:"),
      { target: { value: "1" } }
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Add Track" })
    );

    expect(
      await screen.findByText(
        "Unable to add track to playlist. Please try again."
      )
    ).toBeInTheDocument();
  });

  it("calls onCancel when Cancel is clicked", () => {
    const mockCancel = vi.fn();

    render(
      <AddToPlaylist
        track={track}
        playlists={playlists}
        onCancel={mockCancel}
      />
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Cancel" })
    );

    expect(mockCancel).toHaveBeenCalledTimes(1);
  });
});
