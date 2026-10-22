# SCRUM-97 — Catalog Database Query Review Results

**Project:** Soundwaves  
**Sprint:** 3  
**Owner:** Database workstream  
**Status:** SQL review script prepared; execution results pending  
**Files reviewed:** `001_schema.sql`, `002_dev_seed.sql`, `MusicCatalogService.cs`, `IMusicCatalogService.cs`, `TracksController.cs`, `SongsController.cs`, `CatalogController.cs`, `SoundwavesDbContext.cs`.

## Goal and boundaries

Evaluate Artist-to-Albums, Album-to-Tracks, Track relationships, and Search queries. Verify existing index coverage, avoid unnecessary schema changes, and communicate backend-only issues to the backend owner. No backend source files, migrations, or seed files are changed by this work.

## Test environment

| Item | Record |
|---|---|
| MySQL version (`SELECT VERSION();`) | Pending |
| Docker service/container | Pending |
| Test date | Pending |
| Database | `soundwave` |
| Expected seed counts (fresh database) | 20 artists, 40 albums, 120 tracks |
| Script | `database/scripts/catalog_query_review.sql` |
| Test execution evidence (log/screenshot) | Pending |

## Method

1. Start the project's existing MySQL Docker service and load the schema and seed through the established setup process.
2. Run `catalog_query_review.sql` against the development database, capturing output.
3. For each `EXPLAIN`, record `type`, `key`, estimated `rows`, and `Extra`. Note that estimates and timings are different measurements.
4. Compare actual results with the seeded records. If a query seems inefficient, investigate with representative larger datasets and optionally `EXPLAIN ANALYZE` (MySQL 8.0.18+), which executes the query.
5. Do not add indexes without measured justification. Existing indexes include `idx_albums_artist_id`, `idx_tracks_album_id`, `idx_tracks_artist_id`, and title/name indexes.

## Execution plan and result log

| Query | Expected behavior | `key` used | Estimated rows | Result / observation | Status |
|---|---|---|---|---|---|
| Artist → Albums (artist 6) | Amber Atlas albums | Pending | Pending | Pending | Not run |
| Album → Tracks (album 6) | Three tracks in track-number order | Pending | Pending | Pending | Not run |
| Artist → Tracks (artist 6) | Tracks belonging to artist 6 | Pending | Pending | Pending | Not run |
| Track detail (track 16) | Track, album, and artist metadata | Pending | Pending | Pending | Not run |
| Track title Search (`Midnight`) | Matching track titles | Pending | Pending | Pending | Not run |
| Album title Search (`Midnight`) | Matching album titles | Pending | Pending | Pending | Not run |
| Artist name Search (`Midnight`) | Matching artist names | Pending | Pending | Pending | Not run |
| Referential integrity checks | Zero orphan references | N/A | N/A | Pending | Not run |

## Findings from source review (not execution measurements)

| ID | Finding | Evidence / interpretation | Proposed action | Owner |
|---|---|---|---|---|
| DB-01 | Relationship filters have existing indexes | `albums.artist_id`, `tracks.album_id`, and `tracks.artist_id` indexed in schema; EF Core service uses `Where` and `AsNoTracking()` for relationship reads | Confirm plans; no automatic index additions | Database |
| DB-02 | Basic catalog listing loads all rows | `GetArtistsAsync()` and `GetAlbumsAsync()` call `ToListAsync()` without pagination | Consider paging if catalog grows; coordinate API contract | Backend |
| BE-01 | `/api/tracks` returns hardcoded sample data | `TracksController.GetTracks()` calls service `GetTracks()` which returns `_tracks` | Move listing to database-backed query when backend owner approves | Backend |
| BE-02 | `/api/songs/search` uses hardcoded lists | `SongsController` calls `SearchTracks()`, which filters `_tracks` and `_artists` in memory | Align search data source with database catalog | Backend |
| BE-03 | `/api/catalog/search` issues three category queries | `SearchCatalogAsync()` queries tracks, albums, artists separately | Three queries are not inherently wasteful; measure before combining | Backend |
| BE-04 | Track-by-ID lookup is synchronous and tracked | `GetTrackById()` calls `FirstOrDefault()` without `AsNoTracking()` | Consider async and no-tracking for read-only operation | Backend |
| DB-03 | Substring search may scan title/name indexes | `Contains()`/`LIKE '%term%'` patterns generally cannot use normal B-tree prefix seek | Review plans; preserve substring behavior before considering FULLTEXT | Database + Backend |
| DB-04 | No confirmed N+1 pattern in supplied service | Relationship methods each issue one filtered query; controller/request usage must be profiled to prove repeated calls | Ask backend owner for request traces before asserting N+1 | Backend |

## Optimization decision

**Current recommendation: no schema or index changes.** Existing foreign-key indexes cover the main relationship filters. The expanded development seed is too small to prove meaningful speedups. Revisit only if captured plans and representative workloads demonstrate a bottleneck.

## Backend team handoff (copy into Jira/PR comment)

Reviewed the Sprint 3 Catalog query paths and existing MySQL indexes. Artist-to-Albums, Album-to-Tracks, and Artist-to-Tracks already filter indexed foreign-key columns. No database schema changes are recommended without benchmark evidence. Two API routes currently use hardcoded sample data (`GET /api/tracks` and `GET /api/songs/search`), while `GET /api/catalog/search` queries the database. Please review the data-source inconsistency. Track-by-ID uses a synchronous tracked EF query; consider async/no-tracking. The three database calls in Catalog Search correspond to three result categories and are not automatically a performance defect. SQL execution-plan results will be attached after local Docker testing.

## Evidence checklist

- [ ] Run the script against Docker MySQL
- [ ] Record MySQL version and fresh seed counts
- [ ] Capture `SHOW INDEX` and `EXPLAIN` output
- [ ] Verify sample relationship and Search results
- [ ] Record any measured before/after differences (if changes are made)
- [ ] Share backend handoff notes with teammate
- [ ] Link branch, commit, pull request, and Jira ticket

## Conclusion

Static review is complete. Runtime query plans, timings, and regression evidence remain pending until the script is executed against the development database. No modifications to schema, seed, or backend are currently justified by the evidence available.
