
const API_BASE = "/api/playlists";

async function request(url, options = {}) {
  const response = await fetch(url, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...options.headers,
    },
  });

  if (!response.ok) {
    throw new Error(
      `Playlist API request failed: ${response.status}`
    );
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

export const playlistApi = {
  getAll() {
    return request(API_BASE);
  },

  getById(id) {
    return request(`${API_BASE}/${id}`);
  },

  create(name, description = null) {
    return request(API_BASE, {
      method: "POST",
      body: JSON.stringify({ name, description }),
    });
  },

  update(id, name, description = null) {
    return request(`${API_BASE}/${id}`, {
      method: "PUT",
      body: JSON.stringify({ name, description }),
    });
  },

  delete(id) {
    return request(`${API_BASE}/${id}`, {
      method: "DELETE",
    });
  },

  addTrack(playlistId, trackId) {
    return request(`${API_BASE}/${playlistId}/tracks`, {
      method: "POST",
      body: JSON.stringify({ trackId }),
    });
  },

  removeTrack(playlistId, trackId) {
    return request(
      `${API_BASE}/${playlistId}/tracks/${trackId}`,
      { method: "DELETE" }
    );
  },
};
