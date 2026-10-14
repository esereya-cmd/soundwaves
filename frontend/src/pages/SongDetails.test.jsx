import { afterEach, describe, expect, it, vi } from "vitest";
import {
  cleanup,
  fireEvent,
  render,
  screen,
  waitFor,
} from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import SongDetails from "./SongDetails";

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
});

describe("SongDetails", () => {
  it("displays song details from a mocked API response", async () => {
    const mockSong = {
      id: 1,
      title: "City Lights",
      artist: { name: "Nova Echo" },
      album: { title: "Midnight Dreams" },
    };

    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        status: 200,
        json: async () => mockSong,
      })
    );

    render(<SongDetails songId={1} onBack={() => {}} />);

    expect(
      screen.getByText("Loading song details...")
    ).toBeInTheDocument();

    expect(
      await screen.findByText("City Lights")
    ).toBeInTheDocument();

    expect(
      screen.getByText("Artist: Nova Echo")
    ).toBeInTheDocument();

    expect(
      screen.getByText("Album: Midnight Dreams")
    ).toBeInTheDocument();

    expect(fetch).toHaveBeenCalledWith("/api/tracks/1");
  });

  it("displays an error when the song is not found", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: false,
        status: 404,
      })
    );

    render(<SongDetails songId={999} onBack={() => {}} />);

    expect(
      await screen.findByRole("alert")
    ).toHaveTextContent("Song not found.");
  });

  it("returns to the catalog when Back is clicked", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        status: 200,
        json: async () => ({
          id: 1,
          title: "City Lights",
        }),
      })
    );

    const mockOnBack = vi.fn();

    render(<SongDetails songId={1} onBack={mockOnBack} />);

    await waitFor(() => {
      expect(
        screen.getByText("City Lights")
      ).toBeInTheDocument();
    });

    fireEvent.click(
      screen.getByRole("button", {
        name: "Back to Catalog",
      })
    );

    expect(mockOnBack).toHaveBeenCalledOnce();
  });
});
