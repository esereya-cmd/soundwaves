
import { afterEach, describe, expect, it, vi } from "vitest";
import {
  cleanup,
  fireEvent,
  render,
  screen,
  waitFor,
} from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import CreatePlaylistForm from "./CreatePlaylistForm";

afterEach(() => {
  cleanup();
});

describe("CreatePlaylistForm", () => {
  it("displays the playlist name input and Create button", () => {
    render(<CreatePlaylistForm />);

    expect(
      screen.getByLabelText("Playlist Name:")
    ).toBeInTheDocument();

    expect(
      screen.getByRole("button", { name: "Create Playlist" })
    ).toBeInTheDocument();
  });

  it("shows an error when the playlist name is empty", () => {
    render(<CreatePlaylistForm />);

    fireEvent.click(
      screen.getByRole("button", { name: "Create Playlist" })
    );

    expect(
      screen.getByText("Please enter a playlist name.")
    ).toBeInTheDocument();
  });

  it("submits a valid playlist name", async () => {
    const mockCreatePlaylist = vi.fn().mockResolvedValue();

    render(
      <CreatePlaylistForm onCreatePlaylist={mockCreatePlaylist} />
    );

    fireEvent.change(screen.getByLabelText("Playlist Name:"), {
      target: { value: "My New Playlist" },
    });

    fireEvent.click(
      screen.getByRole("button", { name: "Create Playlist" })
    );

    await waitFor(() => {
      expect(mockCreatePlaylist).toHaveBeenCalledWith(
        "My New Playlist"
      );
    });
  });

  it("prevents submitting again while creating a playlist", async () => {
    let finishRequest;

    const mockCreatePlaylist = vi.fn(
      () =>
        new Promise((resolve) => {
          finishRequest = resolve;
        })
    );

    render(
      <CreatePlaylistForm onCreatePlaylist={mockCreatePlaylist} />
    );

    fireEvent.change(screen.getByLabelText("Playlist Name:"), {
      target: { value: "Chill Mix" },
    });

    fireEvent.click(
      screen.getByRole("button", { name: "Create Playlist" })
    );

    expect(
      screen.getByRole("button", { name: "Creating..." })
    ).toBeDisabled();

    expect(mockCreatePlaylist).toHaveBeenCalledTimes(1);

    finishRequest();

    await waitFor(() => {
      expect(
        screen.getByRole("button", { name: "Create Playlist" })
      ).not.toBeDisabled();
    });
  });

  it("shows an error when playlist creation fails", async () => {
    const mockCreatePlaylist = vi.fn().mockRejectedValue(
      new Error("Request failed")
    );

    render(
      <CreatePlaylistForm onCreatePlaylist={mockCreatePlaylist} />
    );

    fireEvent.change(screen.getByLabelText("Playlist Name:"), {
      target: { value: "My Playlist" },
    });

    fireEvent.click(
      screen.getByRole("button", { name: "Create Playlist" })
    );

    expect(
      await screen.findByText(
        "Unable to create playlist. Please try again."
      )
    ).toBeInTheDocument();
  });
});
