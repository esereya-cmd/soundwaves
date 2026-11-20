
const BASE_URL = "/api/playlists";

// Handle API responses and errors.
async function handleResponse(response) {
  if (!response.ok) {
    let message = "Playlist request failed.";

    if (response.status === 409) {
      message = "This track is already in the playlist.";
    } else if (response.status === 404) {
      message = "Playlist or track not found.";
    } else if (response.status === 401) {
      message = "You are not authorized.";
    } else if (response.status === 400) {
      message = "Invalid playlist information.";
    }

    throw new Error(message);
  }

  // DELETE operations may return no content.
  if (response.status === 204) {
    return null;
  }

  return response.json();
}

// Get all playlists.
export async function getPlaylists() {
  const response = await fetch(BASE_URL);

  return handleResponse(response);
}

// Get one playlist with its tracks.
export async function getPlaylist(id) {
  const response = await fetch(`${BASE_URL}/${id}`);

  return handleResponse(response);
}

// Create a playlist.
export async function createPlaylist(name, description = null) {
  const response = await fetch(BASE_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      name,
      description,
    }),
  });

  return handleResponse(response);
}

// Update a playlist's name and description.
export async function updatePlaylist(id, name, description = null) {
  const response = await fetch(`${BASE_URL}/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      name,
      description,
    }),
  });

  return handleResponse(response);
}

// Delete a playlist.
export async function deletePlaylist(id) {
  const response = await fetch(`${BASE_URL}/${id}`, {
    method: "DELETE",
  });

  return handleResponse(response);
}

// Add a track to a playlist.
export async function addTrack(playlistId, trackId) {
  const response = await fetch(`${BASE_URL}/${playlistId}/tracks`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      trackId,
    }),
  });

  return handleResponse(response);
}

// Remove a track from a playlist.
export async function removeTrack(playlistId, trackId) {
  const response = await fetch(
    `${BASE_URL}/${playlistId}/tracks/${trackId}`,
    {
      method: "DELETE",
    }
  );

  return handleResponse(response);
}
