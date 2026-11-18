
# SCRUM-191 — Soundwaves Playlist E2E Test Fixture

## Purpose

This document explains how to initialize and verify repeatable playlist test data for Soundwaves Sprint 5.

The fixture creates:
- One dedicated QA user: `soundwaves_e2e_qa`
- One playlist: `Sprint 5 QA Playlist`
- One playlist-track association: `Demo Tone A`

The SQL script is located at `backend-database/database/scripts/seed_playlist_e2e.sql`.

This fixture is intended for local development and testing only.

## Prerequisites

- Docker Desktop running
- MySQL container available through Docker Compose
- Soundwaves repository checked out locally
- MySQL credentials configured in `backend-database/.env`
- Existing `Demo Tone A` track with ID `3`

The fixture does not initialize the entire database. The Soundwaves schema and the required track must already exist.

## Application URLs

- React frontend: http://localhost:5173
- ASP.NET Core backend: http://localhost:5072
- MySQL: localhost:3306

## Start MySQL

From the Soundwaves repository root:

```powershell
cd backend-database
docker compose up -d
docker compose ps
```

Verify that `soundwave-mysql` is healthy.

## Copy and Execute the Fixture

From the `backend-database` directory:

```powershell
docker compose cp .\database\scripts\seed_playlist_e2e.sql db:/tmp/seed_playlist_e2e.sql
docker compose exec db mysql -u soundwave -p soundwave
```

Enter the MySQL password when prompted.

At the `mysql>` prompt, execute:

```sql
SOURCE /tmp/seed_playlist_e2e.sql;
```

## Verify the Fixture

Run:

```sql
SELECT
  (SELECT COUNT(*) FROM users
   WHERE username = 'soundwaves_e2e_qa') AS qa_users,
  (SELECT COUNT(*) FROM playlists
   WHERE name = 'Sprint 5 QA Playlist') AS qa_playlists,
  (SELECT COUNT(*)
   FROM playlist_tracks pt
   JOIN playlists p ON p.id = pt.playlist_id
   WHERE p.name = 'Sprint 5 QA Playlist') AS qa_track_links;
```

Expected result:

| qa_users | qa_playlists | qa_track_links |
|---|---|---|
| 1 | 1 | 1 |

## Repeatability

Run the `SOURCE` command again and repeat the verification query.

The fixture should not create additional QA users, playlists, or track associations.

## Known Limitations

- The QA user contains an intentionally invalid password hash and must not be used for authentication testing.
- The existing React playlist views use hardcoded sample data in `App.jsx`.
- A successful SQL fixture does not establish that the playlist frontend is connected to MySQL.
- Full playlist CRUD end-to-end tests remain dependent on the integrated playlist API and frontend implementation.

## Verification Record

- Jira: SCRUM-191
- Tested revision: `3aa9719` (`main`, before fixture commit)
- Fixture commit: `f205234`
- Verified: November 18, 2026
- Result: Initial fixture execution succeeded; repeat execution produced one QA user, one playlist, and one track association.
