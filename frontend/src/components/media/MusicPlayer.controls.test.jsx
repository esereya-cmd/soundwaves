
import { afterEach, describe, expect, it } from "vitest";
import { cleanup, fireEvent, render, screen } from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

import MusicPlayer from "./MusicPlayer";

afterEach(() => {
  cleanup();
});

describe("MusicPlayer controls", () => {
  const sampleTrack = {
    id: 1,
    title: "Sample Track",
  };

  it("shows a message when no track is selected", () => {
    render(<MusicPlayer selectedTrack={null} />);

    expect(
      screen.getByText("No track selected.")
    ).toBeInTheDocument();
  });

  it("changes the audio volume", () => {
    const { container } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    const audio = container.querySelector("audio");
    const volumeSlider = screen.getByLabelText("Volume:");

    fireEvent.change(volumeSlider, {
      target: { value: "0.5" },
    });

    expect(audio.volume).toBe(0.5);
  });

  it("mutes and unmutes the audio", () => {
    const { container } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    const audio = container.querySelector("audio");

    fireEvent.click(
      screen.getByRole("button", { name: "Mute" })
    );

    expect(audio.muted).toBe(true);

    fireEvent.click(
      screen.getByRole("button", { name: "Unmute" })
    );

    expect(audio.muted).toBe(false);
  });

  it("updates the selected track", () => {
    const { rerender } = render(
      <MusicPlayer selectedTrack={sampleTrack} />
    );

    expect(screen.getByText("Sample Track")).toBeInTheDocument();

    rerender(
      <MusicPlayer selectedTrack={{ id: 2, title: "New Track" }} />
    );

    expect(screen.getByText("New Track")).toBeInTheDocument();

    expect(
      screen.queryByText("Sample Track")
    ).not.toBeInTheDocument();
  });
});
