
import { afterEach, describe, expect, it, vi } from "vitest";
import { cleanup, fireEvent, render, screen } from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import MusicPlayer from "./MusicPlayer";

afterEach(() => {
  cleanup();
  vi.restoreAllMocks();
});

describe("MusicPlayer playback", () => {
  const sampleTrack = {
    id: 1,
    title: "Sample Track",
  };

  it("plays audio when the Play button is clicked", () => {
    const { container } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    const audio = container.querySelector("audio");
    const playMock = vi.spyOn(audio, "play").mockResolvedValue();

    fireEvent.click(screen.getByRole("button", { name: "Play" }));

    expect(playMock).toHaveBeenCalledTimes(1);
  });

  it("pauses audio when the Pause button is clicked", () => {
    const { container } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    const audio = container.querySelector("audio");

    Object.defineProperty(audio, "paused", {
      configurable: true,
      value: false,
    });

    const pauseMock = vi.spyOn(audio, "pause").mockImplementation(() => {});

    fireEvent.playing(audio);

    expect(
      screen.getByRole("button", { name: "Pause" })
    ).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Pause" }));

    expect(pauseMock).toHaveBeenCalledTimes(1);

    fireEvent.pause(audio);

    expect(screen.getByText("Status: Paused")).toBeInTheDocument();
  });

  it("resumes audio after pausing", () => {
    const { container } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    const audio = container.querySelector("audio");

    let paused = true;

    Object.defineProperty(audio, "paused", {
      configurable: true,
      get: () => paused,
    });

    const playMock = vi.spyOn(audio, "play").mockImplementation(() => {
      paused = false;
      return Promise.resolve();
    });

    const pauseMock = vi.spyOn(audio, "pause").mockImplementation(() => {
      paused = true;
    });

    fireEvent.click(screen.getByRole("button", { name: "Play" }));
    fireEvent.playing(audio);

    expect(screen.getByText("Status: Playing")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Pause" }));
    fireEvent.pause(audio);

    expect(screen.getByText("Status: Paused")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Play" }));
    fireEvent.playing(audio);

    expect(playMock).toHaveBeenCalledTimes(2);
    expect(pauseMock).toHaveBeenCalledTimes(1);
    expect(screen.getByText("Status: Playing")).toBeInTheDocument();
  });

  it("returns to paused status when playback ends", () => {
    const { container } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    const audio = container.querySelector("audio");

    fireEvent.playing(audio);

    expect(screen.getByText("Status: Playing")).toBeInTheDocument();

    fireEvent.ended(audio);

    expect(screen.getByText("Status: Paused")).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: "Play" })
    ).toBeInTheDocument();
  });
});
