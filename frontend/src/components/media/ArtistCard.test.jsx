
import { describe, it, expect, afterEach } from "vitest";
import { render, screen, cleanup } from "@testing-library/react";
import "@testing-library/jest-dom/vitest";
import ArtistCard from "./ArtistCard";

afterEach(() => {
  cleanup();
});

describe("ArtistCard", () => {
  it("displays the artist name", () => {
    render(<ArtistCard name="Taylor Swift" />);

    expect(screen.getByText("Taylor Swift")).toBeInTheDocument();
  });

  it("displays the artist name as a heading", () => {
    render(<ArtistCard name="Bruno Mars" />);

    expect(
      screen.getByRole("heading", { name: "Bruno Mars" })
    ).toBeInTheDocument();
  });
});
