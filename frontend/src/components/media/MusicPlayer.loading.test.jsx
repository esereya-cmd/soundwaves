
import { afterEach, describe, expect, it, vi } from "vitest";
import { cleanup, fireEvent, render, screen } from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import MusicPlayer from "./MusicPlayer";

afterEach(() => {
  cleanup();
  vi.restoreAllMocks();
});

describe("MusicPlayer loading and errors", () => {
  const sampleTrack = {
    id: 1,
    title: "Sample Track",
  };

  it("shows a loading message when audio starts loading", () => {
    const { container } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    const audio = container.querySelector("audio");

    fireEvent.loadStart(audio);

    expect(screen.getByRole("status")).toHaveTextContent(
      "Loading audio..."
    );
    expect(screen.getByText("Status: Loading")).toBeInTheDocument();
  });

  it("stops showing loading when audio is ready", () => {
    const { container } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    const audio = container.querySelector("audio");

    fireEvent.loadStart(audio);
    fireEvent.canPlay(audio);

    expect(screen.queryByRole("status")).not.toBeInTheDocument();
    expect(screen.getByText("Status: Paused")).toBeInTheDocument();
  });

  it("displays an error when audio fails", () => {
    const { container } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    const audio = container.querySelector("audio");

    fireEvent.error(audio);

    expect(screen.getByText("Status: Error")).toBeInTheDocument();
    expect(screen.getByRole("alert")).toHaveTextContent(
      "Unable to play this track."
    );
    expect(
      screen.getByRole("button", { name: "Retry" })
    ).toBeInTheDocument();
  });

  it("allows the user to retry loading audio", () => {
    const { container } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    const audio = container.querySelector("audio");

    const loadMock = vi
      .spyOn(audio, "load")
      .mockImplementation(() => {});

    fireEvent.error(audio);

    fireEvent.click(
      screen.getByRole("button", { name: "Retry" })
    );

    expect(loadMock).toHaveBeenCalledTimes(1);
    expect(screen.queryByRole("alert")).not.toBeInTheDocument();
    expect(screen.getByRole("status")).toHaveTextContent(
      "Loading audio..."
    );
  });
});
