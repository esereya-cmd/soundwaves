# Soundwaves — Repeatable MySQL Setup and Playlist QA Handoff

**Project:** Soundwaves — CPSC 491  
**Jira Ticket:** SCRUM-189  
**Author:** Teyen Lee  
**Date:** November 19, 2026  
**Database:** MySQL 8.4  
**Environment:** Docker Compose / Adminer / Windows CMD

---

## 1. Purpose

This document provides repeatable instructions for setting up and validating the Soundwaves MySQL development database.

The documentation supports two scenarios:

1. Fresh database initialization.
2. Existing database setup without deleting previously stored data.

It also documents the SCRUM-185 deterministic playlist fixtures and provides verification queries for backend and QA testing.

The objective is to prevent database initialization problems, duplicate fixture records, and mismatches between existing development databases and newer SQL files.

## 2. Prerequisites

Required software:

- Docker Desktop
- Git
- Windows Command Prompt
- Soundwaves GitHub repository
- Adminer (included in Docker Compose)

Repository:

https://github.com/esereya-cmd/soundwaves

Relevant files:

- `backend-database/docker-compose.yml`
- `backend-database/database/init/001_schema.sql`
- `004_playlist_test_fixtures.sql`
- `verify_scrum185.sql`

The last two SQL files must be located using their actual repository paths before running the commands below.

The database uses a Docker-managed persistent volume named `mysql-data`.

The existing local Docker Compose project was identified as `soundwave-database-skeleton`.

## 3. Fresh Database Setup

This procedure applies to a new environment with no existing MySQL data volume.

### Step 1 — Open the database directory

From the repository root:

```cmd
cd backend-database
```

### Step 2 — Configure environment variables

Create a local `.env` file using the project's approved development configuration.

The Docker Compose file reads MySQL credentials and port settings from this file.

Do not commit `.env` or share database passwords.

### Step 3 — Start Docker

```cmd
docker compose -p soundwave-database-skeleton up -d
```

Verify:

```cmd
docker compose -p soundwave-database-skeleton ps
```

Expected:

- `soundwave-mysql` is running and healthy.
- `soundwave-adminer` is running.

### Step 4 — Open Adminer

Open:

http://localhost:8080

This is the default Adminer port unless overridden by `ADMINER_PORT`.

Log in using the configured MySQL credentials.

Select the `soundwave` database.

### Step 5 — Verify schema initialization

The Docker Compose configuration mounts:

```text
./database/init:/docker-entrypoint-initdb.d:ro
```

When MySQL initializes an empty data directory, the entrypoint processes supported initialization files in this directory.

The provided `001_schema.sql` creates the `soundwave` database and its nine tables.

Run:

```sql
SHOW TABLES;
```

Expected tables:

- albums
- artists
- auth_sessions
- favorites
- playback_history
- playlist_tracks
- playlists
- tracks
- users

Expected count: 9 tables.

### Step 6 — Prepare catalog records

The SCRUM-185 playlist fixture script requires catalog track IDs 1, 2, 3, and 4.

Before installing fixtures, load the approved development catalog seed if these records are missing.

Verify:

```sql
SELECT id, title
FROM tracks
WHERE id IN (1,2,3,4)
ORDER BY id;
```

Expected: Four records with IDs 1 through 4.

Do not assume a fresh schema automatically includes catalog records. The supplied `001_schema.sql` creates tables but does not insert tracks.

## 4. Existing Database Setup

This procedure applies when a developer already has a MySQL database containing music metadata, playlists, or other development records.

### Step 1 — Inspect existing containers

```cmd
docker ps -a
```

Look for:

- `soundwave-mysql`
- `soundwave-adminer`

### Step 2 — Reuse the existing Compose project

For the verified local environment:

```cmd
docker compose -p soundwave-database-skeleton ps
```

If the project is stopped:

```cmd
docker compose -p soundwave-database-skeleton up -d
```

The project name should match the existing containers. Other developers should inspect their own Compose project name rather than assuming it is identical.

### Step 3 — Preserve existing records

Do not use:

```cmd
docker compose down -v
```

The `-v` option can delete Docker-managed database volumes.

Do not execute the full `001_schema.sql` against an already-populated database.

The schema uses ordinary `CREATE TABLE` statements, so reapplying it can produce table-already-exists errors.

Before applying a schema migration or fixture update, review its effects and create a backup when existing records need to be preserved.

### Step 4 — Verify the existing schema

In Adminer:

```sql
SHOW TABLES;
SHOW CREATE TABLE playlists;
SHOW CREATE TABLE playlist_tracks;
```

Confirm that the required tables and columns exist.

Apply only approved migrations if the existing database requires changes.

The inspected development database already supports the playlist schema; no additional playlist schema changes were required.

## 5. SCRUM-185 Deterministic Playlist Fixtures

The fixture script is:

`004_playlist_test_fixtures.sql`

This script creates a dedicated development test user and three deterministic playlists.

### Fixture user

Username:

` scrum185_fixture `

The fixture account is disabled and uses a deliberately unusable password hash.

It must not be used for authentication testing.

### Expected playlists

| Playlist | Expected tracks |
|---|---|
| [TEST-S185] Empty Playlist | None |
| [TEST-S185] Basic Playlist | 1, 2, 3 |
| [TEST-S185] Ordered Playlist | 4, 3, 2, 1 |

Expected totals:

- 1 fixture user
- 3 fixture playlists
- 7 playlist memberships

### Fixture installation

Before running the script:

1. Confirm track IDs 1–4 exist.
2. Confirm the target database is a development environment.
3. Check whether the fixture records already exist.
4. Review existing data before rerunning the script.

The script uses a stored procedure, transactions, conflict checks, and conditional inserts.

It is designed to avoid unintended duplicate fixture creation.

It also raises errors when certain unexpected fixture records are detected instead of silently overwriting them.

The script does not create database schema objects permanently.

### Running the fixture script

From Windows CMD, while inside `backend-database`, use the following command if the file is stored in `database/scripts`:

```cmd
docker compose -p soundwave-database-skeleton exec -T db sh -c "exec mysql -u\"$MYSQL_USER\" -p\"$MYSQL_PASSWORD\" \"$MYSQL_DATABASE\"" < database\scripts\004_playlist_test_fixtures.sql
```

Adjust the file path to its actual repository location.

The script uses MySQL client `DELIMITER` commands, so execution through the MySQL command-line client is recommended.

### Important limitation

The fixture installer requires track IDs 1–4. It is not a general catalog migration and does not automatically populate missing catalog records.

Its rerunnable behavior has been reviewed from the SQL implementation. A separate repeated-execution test has not yet been recorded as completed.

For destructive or repeatability testing, prefer an isolated disposable database.

## 6. Database Schema Verification

The actual database schema was inspected using Adminer.

### Playlists table

Important columns:

- `id`
- `user_id`
- `name`
- `description`
- `created_at`
- `updated_at`

Foreign key:

`playlists.user_id` references `users.id`.

Delete behavior:

`ON DELETE CASCADE`

### Playlist tracks table

Important columns:

- `playlist_id`
- `track_id`
- `position`
- `added_at`

Composite primary key:

```sql
PRIMARY KEY (playlist_id, track_id, position)
```

Foreign keys:

- `playlist_id` references `playlists.id`
- `track_id` references `tracks.id`

Both use `ON DELETE CASCADE`.

The composite primary key permits the same track to appear at different positions and does not independently enforce unique positions.

Duplicate membership and ordering rules must therefore be validated against the approved application contract.

## 7. Verification SQL and Results

The following checks were executed against the local Soundwaves development database on November 19, 2026.

### Test 1 — Record counts

```sql
SELECT
    (SELECT COUNT(*) FROM playlists) AS playlist_count,
    (SELECT COUNT(*) FROM playlist_tracks) AS membership_count,
    (SELECT COUNT(*) FROM tracks) AS catalog_track_count;
```

Actual result:

| playlist_count | membership_count | catalog_track_count |
|---:|---:|---:|
| 3 | 7 | 4 |

**Result: PASS**

These values are the observed local baseline, not mandatory counts for every development environment.

### Test 2 — Playlist ownership integrity

```sql
SELECT p.id, p.user_id
FROM playlists p
LEFT JOIN users u ON p.user_id = u.id
WHERE u.id IS NULL;
```

Expected: No rows.

Actual: No rows.

**Result: PASS**

### Test 3 — Orphan membership detection

```sql
SELECT pt.*
FROM playlist_tracks pt
LEFT JOIN playlists p
    ON pt.playlist_id = p.id
LEFT JOIN tracks t
    ON pt.track_id = t.id
WHERE p.id IS NULL OR t.id IS NULL;
```

Expected: No rows.

Actual: No rows.

**Result: PASS**

### Test 4 — Playlist ordering

```sql
SELECT playlist_id, track_id, position
FROM playlist_tracks
ORDER BY playlist_id, position, track_id;
```

Actual result:

| playlist_id | track_id | position |
|---:|---:|---:|
| 3 | 1 | 1 |
| 3 | 2 | 2 |
| 3 | 3 | 3 |
| 4 | 4 | 1 |
| 4 | 3 | 2 |
| 4 | 2 | 3 |
| 4 | 1 | 4 |

**Result: PASS — Current stored ordering is consecutive and contains no duplicate positions.**

API ordering behavior requires separate integration verification.

### Test 5 — Duplicate track membership

```sql
SELECT playlist_id, track_id, COUNT(*) AS occurrences
FROM playlist_tracks
GROUP BY playlist_id, track_id
HAVING COUNT(*) > 1;
```

Expected: No duplicates in the current deterministic fixtures.

Actual: No rows.

**Result: PASS**

### Test 6 — Duplicate positions

```sql
SELECT playlist_id, position, COUNT(*) AS occurrences
FROM playlist_tracks
GROUP BY playlist_id, position
HAVING COUNT(*) > 1;
```

Expected: No duplicate positions in the current fixtures.

Actual: No rows.

**Result: PASS**

## 8. Catalog Fixture Prerequisite Verification

The following query was executed:

```sql
SELECT id, title
FROM tracks
WHERE id IN (1,2,3,4)
ORDER BY id;
```

Actual result:

| ID | Title |
|---:|---|
| 1 | Demo Track |
| 2 | Demo Track |
| 3 | Midnight Drive |
| 4 | Neon Rain |

**Result: PASS**

All four catalog tracks required by SCRUM-185 were present.

## 9. Fixture Installation Verification

The following query verified the installed fixture records:

```sql
SELECT
    u.id AS fixture_user_id,
    u.username,
    p.id AS playlist_id,
    p.name,
    COUNT(pt.track_id) AS track_count
FROM users u
JOIN playlists p ON p.user_id = u.id
LEFT JOIN playlist_tracks pt ON pt.playlist_id = p.id
WHERE u.username = 'scrum185_fixture'
  AND p.name LIKE '[TEST-S185]%'
GROUP BY u.id, u.username, p.id, p.name
ORDER BY p.name;
```

Actual results:

| Playlist ID | Playlist | Track count |
|---:|---|---:|
| 3 | [TEST-S185] Basic Playlist | 3 |
| 2 | [TEST-S185] Empty Playlist | 0 |
| 4 | [TEST-S185] Ordered Playlist | 4 |

Fixture user ID: 2

**Result: PASS**

### Fixture count baseline

```sql
SELECT
    (SELECT COUNT(*)
     FROM users
     WHERE username = 'scrum185_fixture') AS fixture_users,
    (SELECT COUNT(*)
     FROM playlists
     WHERE name LIKE '[TEST-S185]%') AS fixture_playlists,
    (SELECT COUNT(*)
     FROM playlist_tracks pt
     JOIN playlists p ON pt.playlist_id = p.id
     WHERE p.name LIKE '[TEST-S185]%') AS fixture_memberships;
```

Actual result:

| fixture_users | fixture_playlists | fixture_memberships |
|---:|---:|---:|
| 1 | 3 | 7 |

**Result: PASS**

These results confirm that the expected fixture dataset is installed in the inspected local database.

## 10. Backend and QA Handoff

Backend and QA team members should use the documented fixtures when validating playlist operations.

### Recommended test scenarios

| Test | Expected behavior |
|---|---|
| Create playlist | New playlist persists in MySQL |
| Update playlist | Updated metadata persists |
| Delete playlist | Playlist is removed according to approved delete behavior |
| Add track | Membership is created |
| Remove track | Membership is removed |
| Catalog preservation | Removing membership does not delete the underlying catalog track |
| Invalid references | Foreign-key constraints prevent orphan records |
| Duplicate membership | Behavior follows approved application contract |
| Track ordering | API and stored ordering agree |
| Repeat fixture installation | No unintended duplicates or overwrites |

### QA evidence requirements

For each test, record:

- Test environment
- Git commit or branch
- API request
- API response
- SQL query before the operation
- SQL query after the operation
- Expected result
- Actual result
- Pass, Fail, or Blocked status
- Screenshots or test logs

Use actual results rather than assuming success from the database schema.

### Integration limitation

The database-only verification was completed using Docker, MySQL, and Adminer.

Full API-dependent persistence testing requires a running ASP.NET Core backend.

These API-dependent tests must be completed by a teammate with a working backend environment.

## 11. Known Limitations

1. The original Docker initialization directory only runs automatically when MySQL initializes a new data directory.
2. Existing volumes do not automatically receive new schema or seed changes after `git pull`.
3. The SCRUM-185 fixture script requires existing catalog track IDs 1–4.
4. The fixture script has not yet been independently retested for repeated execution.
5. Current database integrity checks do not prove successful backend API integration.
6. The composite membership primary key does not enforce all possible application-level duplicate or ordering restrictions.
7. Fresh database initialization and existing-volume migration still require separate execution evidence.

These limitations should be communicated to backend and QA owners.

## 12. GitHub and QA Evidence

Evidence collected:

- Screenshot of nine existing database tables.
- Screenshots of `SHOW CREATE TABLE` for playlist tables.
- SQL results confirming three playlists, seven memberships, and four tracks.
- Ownership integrity query showing no invalid references.
- Orphan membership query showing no invalid references.
- Track ordering query.
- Duplicate membership query.
- Duplicate position query.
- Catalog prerequisite verification.
- SCRUM-185 fixture playlist verification.
- Fixture count baseline verification.
- Docker container and Compose project inspection.

GitHub pull request: **[Add PR link after creation]**

QA handoff date: **[Add date after sending]**

Backend/QA acknowledgment: **[Add confirmation after review]**

## 13. Conclusion

The inspected Soundwaves MySQL development database contains the required playlist schema and deterministic SCRUM-185 test fixtures.

The recorded SQL verification results confirm valid ownership relationships, valid membership references, consistent stored ordering, and no unexpected duplicates in the inspected dataset.

The documentation provides separate procedures for fresh and existing databases, identifies the required fixture prerequisites, and explains how to preserve existing data.

Remaining integration verification and any unexecuted setup scenarios must be recorded separately by the responsible backend and QA testers.

**SCRUM-189 documentation prepared for GitHub review and QA handoff.**
