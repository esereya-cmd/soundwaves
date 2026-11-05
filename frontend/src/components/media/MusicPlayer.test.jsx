
import { afterEach, describe, expect, it } from "vitest";
import { cleanup, fireEvent, render, screen } from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import MusicPlayer from "./MusicPlayer";

afterEach(() => {
  cleanup();
});

describe("MusicPlayer", () => {
  const sampleTrack = {
    id: 1,
    title: "Sample Track",
  };

  it("displays the selected track and initial time", () => {
    render(<MusicPlayer selectedTrack={sampleTrack} />);

    expect(screen.getByText("Sample Track")).toBeInTheDocument();
    expect(screen.getByText("0:00 / 0:00")).toBeInTheDocument();
    expect(screen.getByLabelText("Seek:")).toBeDisabled();
  });

  it("updates the current time and duration", () => {
    const { container } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    const audio = container.querySelector("audio");

    Object.defineProperty(audio, "duration", {
      configurable: true,
      value: 180,
    });

    Object.defineProperty(audio, "currentTime", {
      configurable: true,
      writable: true,
      value: 20,
    });

    fireEvent.loadedMetadata(audio);
    fireEvent.timeUpdate(audio);

    expect(screen.getByText("0:20 / 3:00")).toBeInTheDocument();
    expect(screen.getByLabelText("Seek:")).not.toBeDisabled();
  });

  it("allows seeking to a new time", () => {
    const { container } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    const audio = container.querySelector("audio");

    Object.defineProperty(audio, "duration", {
      configurable: true,
      value: 180,
    });

    Object.defineProperty(audio, "currentTime", {
      configurable: true,
      writable: true,
      value: 0,
    });

    fireEvent.loadedMetadata(audio);

    const seekBar = screen.getByLabelText("Seek:");

    fireEvent.change(seekBar, {
      target: { value: "75" },
    });

    expect(audio.currentTime).toBe(75);
    expect(screen.getByText("1:15 / 3:00")).toBeInTheDocument();
  });
});
