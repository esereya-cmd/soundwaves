
import { describe, it, expect, afterEach } from "vitest";
import {
  render,
  screen,
  cleanup,
} from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import AlbumCard from "./AlbumCard";

afterEach(() => {
  cleanup();
});

describe("AlbumCard", () => {
  // Test 1: Check album title
  it("displays the album title", () => {
    render(
      <AlbumCard
        title="City Lights"
        artist="Nova Echo"
      />
    );

    expect(
      screen.getByText("City Lights")
    ).toBeInTheDocument();
  });

  // Test 2: Check artist name
  it("displays the artist name", () => {
    render(
      <AlbumCard
        title="City Lights"
        artist="Nova Echo"
      />
    );

    expect(
      screen.getByText("Nova Echo")
    ).toBeInTheDocument();
  });
});
