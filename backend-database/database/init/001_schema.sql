-- Soundwave database development schema
-- MySQL 8.x
-- Keep this file focused on the minimum schema needed for local development.

CREATE DATABASE IF NOT EXISTS soundwave
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;

USE soundwave;

CREATE TABLE users (
    id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(64) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role ENUM('user', 'admin') NOT NULL DEFAULT 'user',
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE artists (
    id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_artists_name (name)
) ENGINE=InnoDB;

CREATE TABLE albums (
    id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    artist_id BIGINT UNSIGNED NULL,
    title VARCHAR(255) NOT NULL,
    release_date DATE NULL,
    artwork_path VARCHAR(1024) NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_albums_artist
      FOREIGN KEY (artist_id) REFERENCES artists(id)
      ON DELETE SET NULL,
    INDEX idx_albums_title (title),
    INDEX idx_albums_artist_id (artist_id)
) ENGINE=InnoDB;

CREATE TABLE tracks (
    id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    album_id BIGINT UNSIGNED NULL,
    artist_id BIGINT UNSIGNED NULL,
    title VARCHAR(255) NOT NULL,
    duration_ms INT UNSIGNED NULL,
    track_number SMALLINT UNSIGNED NULL,
    file_path VARCHAR(1024) NOT NULL,
    mime_type VARCHAR(128) NULL,
    file_size_bytes BIGINT UNSIGNED NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_tracks_album
      FOREIGN KEY (album_id) REFERENCES albums(id)
      ON DELETE SET NULL,
    CONSTRAINT fk_tracks_artist
      FOREIGN KEY (artist_id) REFERENCES artists(id)
      ON DELETE SET NULL,
    INDEX idx_tracks_title (title),
    INDEX idx_tracks_album_id (album_id),
    INDEX idx_tracks_artist_id (artist_id)
) ENGINE=InnoDB;

CREATE TABLE playlists (
    id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    user_id BIGINT UNSIGNED NOT NULL,
    name VARCHAR(255) NOT NULL,
    description VARCHAR(1000) NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT fk_playlists_user
      FOREIGN KEY (user_id) REFERENCES users(id)
      ON DELETE CASCADE,
    INDEX idx_playlists_user_id (user_id)
) ENGINE=InnoDB;

CREATE TABLE playlist_tracks (
    playlist_id BIGINT UNSIGNED NOT NULL,
    track_id BIGINT UNSIGNED NOT NULL,
    position INT UNSIGNED NOT NULL,
    added_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (playlist_id, track_id, position),
    CONSTRAINT fk_playlist_tracks_playlist
      FOREIGN KEY (playlist_id) REFERENCES playlists(id)
      ON DELETE CASCADE,
    CONSTRAINT fk_playlist_tracks_track
      FOREIGN KEY (track_id) REFERENCES tracks(id)
      ON DELETE CASCADE,
    INDEX idx_playlist_tracks_track_id (track_id)
) ENGINE=InnoDB;

CREATE TABLE favorites (
    user_id BIGINT UNSIGNED NOT NULL,
    track_id BIGINT UNSIGNED NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, track_id),
    CONSTRAINT fk_favorites_user
      FOREIGN KEY (user_id) REFERENCES users(id)
      ON DELETE CASCADE,
    CONSTRAINT fk_favorites_track
      FOREIGN KEY (track_id) REFERENCES tracks(id)
      ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE playback_history (
    id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    user_id BIGINT UNSIGNED NOT NULL,
    track_id BIGINT UNSIGNED NOT NULL,
    event_type ENUM('play', 'pause', 'skip', 'complete') NOT NULL DEFAULT 'play',
    position_ms INT UNSIGNED NULL,
    played_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_playback_history_user
      FOREIGN KEY (user_id) REFERENCES users(id)
      ON DELETE CASCADE,
    CONSTRAINT fk_playback_history_track
      FOREIGN KEY (track_id) REFERENCES tracks(id)
      ON DELETE CASCADE,
    INDEX idx_playback_history_user_played_at (user_id, played_at),
    INDEX idx_playback_history_track_id (track_id)
) ENGINE=InnoDB;

CREATE TABLE auth_sessions (
    id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    user_id BIGINT UNSIGNED NOT NULL,
    refresh_token_hash CHAR(64) NOT NULL UNIQUE,
    expires_at DATETIME NOT NULL,
    revoked_at DATETIME NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_auth_sessions_user
      FOREIGN KEY (user_id) REFERENCES users(id)
      ON DELETE CASCADE,
    INDEX idx_auth_sessions_user_id (user_id),
    INDEX idx_auth_sessions_expires_at (expires_at)
) ENGINE=InnoDB;
