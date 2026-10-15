
import { afterEach, describe, expect, it, vi } from "vitest";
import {
  cleanup,
  fireEvent,
  render,
  screen,
} from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import ArtistDetails from "./ArtistDetails";

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
});

describe("ArtistDetails", () => {
  it("displays artist details, albums, and tracks", async () => {
    const mockArtist = {
      id: 1,
      name: "Nova Echo",
      albums: [
        { id: 1, title: "City Lights" },
        { id: 2, title: "Midnight Dreams" },
      ],
      tracks: [
        { id: 1, title: "Night Drive" },
        { id: 2, title: "Ocean Blue" },
      ],
    };

    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        status: 200,
        json: async () => mockArtist,
      })
    );

    render(<ArtistDetails artistId={1} onBack={() => {}} />);

    expect(
      await screen.findByText("Nova Echo")
    ).toBeInTheDocument();

    expect(screen.getByText("City Lights")).toBeInTheDocument();
    expect(screen.getByText("Midnight Dreams")).toBeInTheDocument();
    expect(screen.getByText("Night Drive")).toBeInTheDocument();
    expect(screen.getByText("Ocean Blue")).toBeInTheDocument();

    expect(fetch).toHaveBeenCalledWith("/api/artists/1");
  });

  it("displays an error when artist is not found", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: false,
        status: 404,
      })
    );

    render(<ArtistDetails artistId={999} onBack={() => {}} />);

    expect(
      await screen.findByRole("alert")
    ).toHaveTextContent("Artist not found.");
  });

  it("returns to catalog when Back is clicked", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        status: 200,
        json: async () => ({
          id: 1,
          name: "Nova Echo",
          albums: [],
          tracks: [],
        }),
      })
    );

    const mockOnBack = vi.fn();

    render(<ArtistDetails artistId={1} onBack={mockOnBack} />);

    await screen.findByText("Nova Echo");

    fireEvent.click(
      screen.getByRole("button", { name: "Back to Catalog" })
    );

    expect(mockOnBack).toHaveBeenCalledOnce();
  });
});
