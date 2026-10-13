import { afterEach, describe, expect, it, vi } from "vitest";
import {
  cleanup,
  fireEvent,
  render,
  screen,
} from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import AlbumDetails from "./AlbumDetails";

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
});

describe("AlbumDetails", () => {
  it("displays album details and tracks", async () => {
    const mockAlbum = {
      id: 1,
      title: "Midnight Dreams",
      artist: { name: "Nova Echo" },
      tracks: [
        { id: 1, title: "City Lights" },
        { id: 2, title: "Night Drive" },
      ],
    };

    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        status: 200,
        json: async () => mockAlbum,
      })
    );

    render(<AlbumDetails albumId={1} onBack={() => {}} />);

    expect(
      await screen.findByText("Midnight Dreams")
    ).toBeInTheDocument();

    expect(
      screen.getByText("Artist: Nova Echo")
    ).toBeInTheDocument();

    expect(screen.getByText("City Lights")).toBeInTheDocument();
    expect(screen.getByText("Night Drive")).toBeInTheDocument();

    expect(fetch).toHaveBeenCalledWith("/api/albums/1");
  });

  it("displays an error when album is not found", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: false,
        status: 404,
      })
    );

    render(<AlbumDetails albumId={999} onBack={() => {}} />);

    expect(
      await screen.findByRole("alert")
    ).toHaveTextContent("Album not found.");
  });

  it("returns to catalog when Back is clicked", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        status: 200,
        json: async () => ({
          id: 1,
          title: "Midnight Dreams",
          tracks: [],
        }),
      })
    );

    const mockOnBack = vi.fn();

    render(<AlbumDetails albumId={1} onBack={mockOnBack} />);

    await screen.findByText("Midnight Dreams");

    fireEvent.click(
      screen.getByRole("button", { name: "Back to Catalog" })
    );

    expect(mockOnBack).toHaveBeenCalledOnce();
  });
});