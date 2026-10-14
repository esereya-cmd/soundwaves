
import { describe, it, expect, vi, afterEach } from "vitest";
import {
  render,
  screen,
  fireEvent,
  cleanup,
} from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import SongCard from "./SongCard";

afterEach(() => {
  cleanup();
});

describe("SongCard", () => {
  it("displays the song title", () => {
    render(<SongCard title="Test Song" />);

    expect(
      screen.getByText("Test Song")
    ).toBeInTheDocument();
  });

  it("displays the artist ID and album ID", () => {
    render(
      <SongCard
        title="Test Song"
        artistId={1}
        albumId={2}
      />
    );

    expect(
      screen.getByText("Artist ID: 1")
    ).toBeInTheDocument();

    expect(
      screen.getByText("Album ID: 2")
    ).toBeInTheDocument();
  });

  it("calls onSelect when View Details is clicked", () => {
    const mockOnSelect = vi.fn();

    const { getByRole } = render(
      <SongCard
        title="Test Song"
        onSelect={mockOnSelect}
      />
    );

    fireEvent.click(
      getByRole("button", { name: "View Details" })
    );

    expect(mockOnSelect).toHaveBeenCalledOnce();
  });
});
