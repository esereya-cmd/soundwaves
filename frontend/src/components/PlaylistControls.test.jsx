
import { afterEach, describe, expect, it, vi } from "vitest";
import {
  cleanup,
  fireEvent,
  render,
  screen,
  waitFor,
} from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import PlaylistControls from "./PlaylistControls";

afterEach(() => {
  cleanup();
  vi.restoreAllMocks();
});

describe("PlaylistControls", () => {
  const playlist = {
    id: 1,
    name: "My Favorites",
    tracks: [],
  };

  it("displays rename and delete controls", () => {
    render(<PlaylistControls playlist={playlist} />);

    expect(
      screen.getByLabelText("Playlist Name:")
    ).toHaveValue("My Favorites");

    expect(
      screen.getByRole("button", { name: "Rename Playlist" })
    ).toBeInTheDocument();

    expect(
      screen.getByRole("button", { name: "Delete Playlist" })
    ).toBeInTheDocument();
  });

  it("shows an error for an empty playlist name", () => {
    render(<PlaylistControls playlist={playlist} />);

    fireEvent.change(screen.getByLabelText("Playlist Name:"), {
      target: { value: "   " },
    });

    fireEvent.click(
      screen.getByRole("button", { name: "Rename Playlist" })
    );

    expect(
      screen.getByText("Please enter a playlist name.")
    ).toBeInTheDocument();
  });

  it("submits a new playlist name", async () => {
    const mockRename = vi.fn().mockResolvedValue();

    render(
      <PlaylistControls
        playlist={playlist}
        onRename={mockRename}
      />
    );

    fireEvent.change(screen.getByLabelText("Playlist Name:"), {
      target: { value: "New Favorites" },
    });

    fireEvent.click(
      screen.getByRole("button", { name: "Rename Playlist" })
    );

    await waitFor(() => {
      expect(mockRename).toHaveBeenCalledWith(1, "New Favorites");
    });
  });

  it("does not delete when the user cancels", () => {
    const mockDelete = vi.fn();

    vi.spyOn(window, "confirm").mockReturnValue(false);

    render(
      <PlaylistControls
        playlist={playlist}
        onDelete={mockDelete}
      />
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Delete Playlist" })
    );

    expect(mockDelete).not.toHaveBeenCalled();
  });

  it("deletes the playlist after confirmation", async () => {
    const mockDelete = vi.fn().mockResolvedValue();

    vi.spyOn(window, "confirm").mockReturnValue(true);

    render(
      <PlaylistControls
        playlist={playlist}
        onDelete={mockDelete}
      />
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Delete Playlist" })
    );

    await waitFor(() => {
      expect(mockDelete).toHaveBeenCalledWith(1);
    });
  });

  it("shows an error if renaming fails", async () => {
    const mockRename = vi.fn().mockRejectedValue(
      new Error("Request failed")
    );

    render(
      <PlaylistControls
        playlist={playlist}
        onRename={mockRename}
      />
    );

    fireEvent.change(screen.getByLabelText("Playlist Name:"), {
      target: { value: "New Favorites" },
    });

    fireEvent.click(
      screen.getByRole("button", { name: "Rename Playlist" })
    );

    expect(
      await screen.findByText(
        "Unable to rename playlist. Please try again."
      )
    ).toBeInTheDocument();
  });

  it("shows an error if deleting fails", async () => {
    const mockDelete = vi.fn().mockRejectedValue(
      new Error("Request failed")
    );

    vi.spyOn(window, "confirm").mockReturnValue(true);

    render(
      <PlaylistControls
        playlist={playlist}
        onDelete={mockDelete}
      />
    );

    fireEvent.click(
      screen.getByRole("button", { name: "Delete Playlist" })
    );

    expect(
      await screen.findByText(
        "Unable to delete playlist. Please try again."
      )
    ).toBeInTheDocument();
  });
});
