# Soundwaves Playlist API Contract

**Jira:** SCRUM-174 â€” Define Playlist Routes, DTOs, and HTTP Status Contract
**Sprint:** 5
**Component:** ASP.NET Core Backend
**Status:** Implementation Contract â€” Team Review Required

## 1. Purpose

Define a consistent and secure HTTP API contract for managing personal playlists in the Soundwaves music streaming application.

This contract establishes the endpoints, request and response DTOs, validation requirements, HTTP status codes, playlist ownership rules, and frontend integration requirements for playlist management.

The implementation will follow the existing ASP.NET Core backend conventions and use the established MySQL database schema.

The contract supports the existing React playlist components while preserving working music catalog, streaming, and database functionality.

## 2. Playlist API Endpoints

**Base Route:** `/api/playlists`

| Method | Route | Purpose | Jira |
|---|---|---|---|
| POST | `/api/playlists` | Create playlist | SCRUM-175 |
| GET | `/api/playlists` | List current user's playlists | SCRUM-175 |
| GET | `/api/playlists/{id}` | Retrieve playlist details | SCRUM-176 |
| PUT | `/api/playlists/{id}` | Update playlist information | SCRUM-176 |
| DELETE | `/api/playlists/{id}` | Delete playlist | SCRUM-177 |
| POST | `/api/playlists/{id}/tracks` | Add track to playlist | SCRUM-178 |
| DELETE | `/api/playlists/{id}/tracks/{trackId}` | Remove track from playlist | SCRUM-179 |
| PUT | `/api/playlists/{id}/tracks/reorder` | Reorder playlist tracks | SCRUM-179 |

Playlist and track identifiers use unsigned 64-bit integers (`ulong`), consistent with existing backend conventions.

All playlist operations enforce ownership through the server-controlled identity mechanism.

These routes define the implementation contract. They are not represented as implemented until the corresponding backend work is completed and verified.

## 3. Playlist CRUD Operations

### 3.1 Create Playlist

**Method:** POST
**Route:** `/api/playlists`

Example request:

```json
{
  "name": "My Favorites",
  "description": "My favorite songs"
}
```

**Request DTO:**

| Field | Type | Required | Validation |
|---|---|---|---|
| name | string | Yes | Trimmed, nonempty, maximum 255 characters |
| description | string or null | No | Maximum 1,000 characters |

The backend determines the owning user from the server-side identity provider.

Clients must not supply `user_id` as proof of playlist ownership.

**Successful Response: HTTP 201 Created**

```json
{
  "id": 100,
  "name": "My Favorites",
  "description": "My favorite songs",
  "tracks": []
}
```

The response includes a `Location` header referencing the newly created playlist:

```http
Location: /api/playlists/100
```

The playlist is stored in the existing MySQL `playlists` table.

### 3.2 List Playlists

**Method:** GET
**Route:** `/api/playlists`

Returns playlists belonging to the current user.

**Successful Response: HTTP 200 OK**

```json
[
  {
    "id": 100,
    "name": "My Favorites",
    "description": "My favorite songs"
  },
  {
    "id": 101,
    "name": "Study Music",
    "description": null
  }
]
```

Only playlists owned by the current user are returned.

If the user has no playlists, the endpoint returns an empty array:

```json
[]
```

The response does not include playlists belonging to other users.

### 3.3 Get Playlist Details

**Method:** GET
**Route:** `/api/playlists/{id}`

Retrieves playlist information and its associated tracks.

**Successful Response: HTTP 200 OK**

```json
{
  "id": 100,
  "name": "My Favorites",
  "description": "My favorite songs",
  "tracks": [
    {
      "id": 1,
      "title": "Example Track",
      "position": 1
    },
    {
      "id": 2,
      "title": "Another Track",
      "position": 2
    }
  ]
}
```

Tracks are returned in ascending playlist position order.

The response maintains compatibility with the existing React frontend, which expects playlist objects containing `id`, `name`, and a `tracks` array.

Each track includes at least `id` and `title`.

A playlist that does not exist or does not belong to the current user returns HTTP 404 Not Found.

### 3.4 Update Playlist

**Method:** PUT
**Route:** `/api/playlists/{id}`

Updates the playlist name and description.

Example request:

```json
{
  "name": "Updated Favorites",
  "description": "Updated playlist description"
}
```

Both `name` and `description` fields are included in the request. The description may be null.

**Successful Response: HTTP 200 OK**

```json
{
  "id": 100,
  "name": "Updated Favorites",
  "description": "Updated playlist description"
}
```

The backend validates the input and verifies ownership before updating the playlist.

Updating playlist metadata does not modify its tracks.

### 3.5 Delete Playlist

**Method:** DELETE
**Route:** `/api/playlists/{id}`

Deletes a playlist belonging to the current user.

**Successful Response: HTTP 204 No Content**

The existing MySQL foreign key configuration automatically removes associated records from `playlist_tracks` through `ON DELETE CASCADE`.

Deleting a playlist does not delete the underlying music catalog tracks.

Attempts to delete nonexistent or inaccessible playlists return HTTP 404 Not Found.

## 4. Playlist Track Management

### 4.1 Add Track

**Method:** POST
**Route:** `/api/playlists/{id}/tracks`

Adds an existing music catalog track to a playlist.

Example request:

```json
{
  "trackId": 1
}
```

**Request DTO:**

| Field | Type | Required | Validation |
|---|---|---|---|
| trackId | ulong | Yes | Positive, existing track identifier |

**Successful Response: HTTP 200 OK**

```json
{
  "id": 1,
  "title": "Example Track",
  "position": 1
}
```

Before adding a track, the backend verifies:

1. The playlist exists.
2. The playlist belongs to the current user.
3. The track exists in the music catalog.
4. The track is not already present in the playlist.
5. The new position follows the established ordering rules.

New tracks are appended to the end of the playlist.

If the playlist currently contains three tracks, the new track receives position 4.

**Duplicate Track Behavior:**

A track may appear only once within a playlist.

Attempting to add an existing playlist track returns:

**HTTP 409 Conflict**

The existing association remains unchanged.

### 4.2 Remove Track

**Method:** DELETE
**Route:** `/api/playlists/{id}/tracks/{trackId}`

Removes an existing track association from a playlist.

**Successful Response: HTTP 204 No Content**

The underlying music catalog track remains unchanged.

After removing a track, the remaining playlist positions are updated to maintain consecutive numbering.

For example:

Before removal:

| Track | Position |
|---|---|
| Track A | 1 |
| Track B | 2 |
| Track C | 3 |

After removing Track B:

| Track | Position |
|---|---|
| Track A | 1 |
| Track C | 2 |

Removing a track that is not associated with the playlist returns HTTP 404 Not Found.

### 4.3 Reorder Tracks

**Method:** PUT
**Route:** `/api/playlists/{id}/tracks/reorder`

Updates the order of tracks in an existing playlist.

Example request:

```json
{
  "trackIds": [3, 1, 2]
}
```

**Successful Response: HTTP 204 No Content**

The backend assigns consecutive positions based on the submitted order.

For the example above:

| Track ID | New Position |
|---|---|
| 3 | 1 |
| 1 | 2 |
| 2 | 3 |

The reorder request must contain exactly the existing playlist track IDs, with no duplicates or omissions.

An empty array is valid only when the playlist contains no tracks.

Invalid reorder requests return HTTP 400 Bad Request.

Reordering does not create or delete tracks.

Position updates must be handled consistently to prevent partial or conflicting playlist states.

## 5. HTTP Status Codes and Error Handling

| Status | Condition |
|---|---|
| 200 OK | Successful retrieval, update, or track addition |
| 201 Created | Playlist successfully created |
| 204 No Content | Successful playlist deletion, track removal, or reorder |
| 400 Bad Request | Invalid identifiers, malformed requests, or validation failures |
| 401 Unauthorized | No valid server-side identity is available |
| 403 Forbidden | Authenticated identity lacks permission where applicable |
| 404 Not Found | Requested playlist or track is missing or inaccessible |
| 409 Conflict | Attempt to add a duplicate track or another conflicting operation |
| 500 Internal Server Error | Unexpected server failure handled without exposing sensitive information |

### 5.1 Validation Error Response

The backend uses a consistent ASP.NET Core Problem Details response format.

Example:

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Bad Request",
  "status": 400,
  "detail": "Playlist name cannot be empty."
}
```

This is an illustrative contract response, not an observed backend result.

### 5.2 Ownership Error Handling

Requests for playlists belonging to another user return HTTP 404 Not Found.

This prevents the API from disclosing whether another user's playlist exists.

### 5.3 Internal Error Handling

Unexpected failures must not expose:

- Database connection strings.
- Internal exception details.
- Server filesystem paths.
- Authentication credentials.
- Sensitive user information.

## 6. Validation Requirements

Playlist validation enforces the following rules:

1. Playlist names cannot be null, empty, or whitespace-only.
2. Playlist names are trimmed before storage.
3. Playlist names cannot exceed 255 characters after trimming.
4. Playlist descriptions cannot exceed 1,000 characters.
5. Playlist identifiers must be positive.
6. Track identifiers must be positive and reference existing tracks.
7. All playlist operations verify ownership.
8. Duplicate tracks within a playlist are rejected.
9. Track positions begin at 1.
10. Track positions remain consecutive after additions, removals, and reordering.
11. Reorder requests contain the complete set of existing playlist track IDs.
12. Invalid requests return controlled and consistent HTTP errors.

Playlist names are not required to be unique.

The implementation must preserve the existing database schema unless a verified requirement necessitates changes.

## 7. Playlist Ownership and Development Identity

The existing backend does not currently provide a complete authentication mechanism.

For Sprint 5 local development and demonstration, Soundwaves uses a designated development-only demo account.

### 7.1 Development Demo Account

| Field | Value |
|---|---|
| Database | `soundwave` |
| User ID | `1001` |
| Username | `soundwaves_demo` |
| Email | `demo@soundwaves.local` |
| Role | `user` |
| Active | `1` |

The account was created and verified in a local MySQL development database.

Other developers must reproduce the account in their own development environments.

### 7.2 Development Identity Configuration

The backend will use a server-controlled development identity mechanism.

Requirements:

- The fixed demo identity is enabled only in the Development environment.
- The development identity resolves to user ID 1001.
- Clients cannot supply arbitrary user IDs to bypass ownership checks.
- Production environments must not use the fixed demo identity.
- Playlist operations fail closed when no valid identity is available.
- Automated tests may use a controlled test identity through an isolated testing mechanism.

The development identity mechanism must be implemented and verified under the appropriate backend ticket.

### 7.3 Demo Account Seeding

A repeatable development-only database seed process will establish the demo account.

The process must:

1. Be safe to execute repeatedly.
2. Avoid overwriting existing users.
3. Detect conflicting IDs, usernames, and email addresses.
4. Avoid usable authentication credentials.
5. Never run automatically in production.
6. Document the setup procedure for other team members.

The seed process will be completed during backend implementation.

## 8. Database Schema Compatibility

The existing MySQL schema already supports playlist storage.

### 8.1 Playlists Table

**Table:** `playlists`

| Column | Type | Purpose |
|---|---|---|
| id | BIGINT UNSIGNED | Playlist identifier |
| user_id | BIGINT UNSIGNED | Playlist owner |
| name | VARCHAR(255) | Playlist name |
| description | VARCHAR(1000) | Optional description |
| created_at | TIMESTAMP | Creation timestamp |
| updated_at | TIMESTAMP | Last update timestamp |

The `user_id` column references the existing `users` table.

### 8.2 Playlist Tracks Table

**Table:** `playlist_tracks`

| Column | Type | Purpose |
|---|---|---|
| playlist_id | BIGINT UNSIGNED | Associated playlist |
| track_id | BIGINT UNSIGNED | Associated catalog track |
| position | INT UNSIGNED | Track order |
| added_at | TIMESTAMP | Association timestamp |

The table uses a composite primary key:

`(playlist_id, track_id, position)`

The schema does not independently enforce unique positions or prohibit duplicate track IDs within a playlist.

These restrictions are enforced by the backend service layer.

### 8.3 Deletion and Referential Integrity

The existing foreign key constraints support cascading deletion of playlist-track associations when a playlist is deleted.

Deleting a playlist must not remove records from the music catalog.

The implementation will use the existing Entity Framework Core conventions for MySQL table and column mappings.

No database schema changes are required by this contract.

## 9. Frontend Integration

Existing React playlist components include:

- `CreatePlaylistForm.jsx`
- `PlaylistControls.jsx`
- `PlaylistList.jsx`
- `PlaylistDetails.jsx`

These components support playlist creation, selection, renaming, deletion, and displaying associated tracks.

The frontend expects playlist objects containing `id` and `name`.

Playlist details include a `tracks` array containing track objects with `id` and `title`.

The API responses defined in this contract preserve those requirements.

The frontend integration layer will connect the existing component callbacks to the backend endpoints.

Frontend and backend developers must use the same request and response formats.

Integration testing must confirm that playlist operations persist correctly in MySQL and remain available after refreshing the application.

## 10. Testing Strategy

Playlist backend implementation and validation are distributed across the following Sprint 5 tickets.

| Jira | Responsibility |
|---|---|
| SCRUM-175 | Create and list playlist endpoints |
| SCRUM-176 | Retrieve and update playlist endpoints |
| SCRUM-177 | Delete playlist and cleanup behavior |
| SCRUM-178 | Add track to playlist |
| SCRUM-179 | Remove and reorder tracks |
| SCRUM-180 | Playlist service validation and unit tests |
| SCRUM-181 | Backend build and test checks in CI |

### 10.1 Automated Backend Tests

The test suite will cover:

- Creating valid playlists.
- Rejecting empty or invalid playlist names.
- Listing only the current user's playlists.
- Retrieving existing playlists.
- Updating playlist metadata.
- Deleting playlists and associated relationships.
- Adding valid catalog tracks.
- Rejecting nonexistent tracks.
- Rejecting duplicate playlist tracks with HTTP 409.
- Removing playlist tracks.
- Maintaining consecutive track positions.
- Reordering playlist tracks.
- Rejecting incomplete or duplicate reorder requests.
- Enforcing playlist ownership isolation.
- Rejecting requests without an authorized identity.
- Verifying development identity restrictions.

### 10.2 Integration Testing

Integration testing will verify that playlist API operations correctly interact with the existing database.

The tests must confirm that playlist creation, modification, track management, and deletion persist as expected.

Existing catalog and streaming functionality must remain operational.

### 10.3 Continuous Integration

SCRUM-181 will integrate the playlist backend build and automated tests into the existing GitHub Actions workflow.

CI must verify the backend builds successfully and the relevant automated tests pass.

Results must be confirmed through actual workflow execution.

## 11. Acceptance Criteria

The playlist API contract defines the following documentation and implementation requirements:

- [x] Canonical playlist API routes are documented.
- [x] Request and response DTO structures are defined.
- [x] HTTP success and error responses are documented.
- [x] Playlist validation rules are documented.
- [x] Playlist ownership requirements are specified.
- [x] Development-only identity restrictions are documented.
- [x] Database schema compatibility is reviewed.
- [x] Duplicate-track behavior is defined.
- [x] Track position numbering is defined.
- [x] Reorder behavior is documented.
- [ ] Frontend and backend owners complete contract review.
- [ ] Official SCRUM-174 Jira acceptance criteria are verified.
- [ ] The contract is linked to SCRUM-174 in Jira.

The checked items indicate coverage within this document, not verified backend implementation or formal team acceptance.

## 12. Playlist Design Decisions

The following design decisions establish consistent behavior for the Soundwaves playlist backend implementation.

| Decision | Implementation Rule |
|---|---|
| Duplicate tracks | A track may appear only once within a playlist. Adding an existing track returns HTTP 409 Conflict. |
| Position numbering | Track positions begin at 1 and remain consecutive. |
| Reorder format | The client submits the complete ordered array of track IDs. |
| Development identity | The backend uses server-controlled user ID 1001 exclusively in the Development environment. |
| Demo account seeding | An idempotent development-only seed script creates the demo account without overwriting existing users. |
| PUT semantics | Playlist updates require the name and description fields. The description may be null. |
| Add-track response | Successfully adding a track returns HTTP 200 OK with the added track DTO. |
| Missing track association | Removing a track that is not in the playlist returns HTTP 404 Not Found. |

### 12.1 Duplicate Track Prevention

Each playlist contains unique track IDs.

If a user attempts to add a track already associated with the playlist, the backend returns HTTP 409 Conflict.

This behavior simplifies track removal, ordering, and frontend integration.

### 12.2 Track Position Management

Track positions use consecutive, one-based numbering.

For example, a playlist containing four tracks uses positions:

`1, 2, 3, 4`

If the second track is removed, the remaining tracks are renumbered:

`1, 2, 3`

Reordering updates positions while preserving the same set of tracks.

### 12.3 Playlist Ownership

All playlist operations are restricted to the current user's playlists.

During local development, the backend uses the designated development-only demo identity.

The fixed identity must never provide an authentication bypass in production.

### 12.4 Implementation Consistency

These design rules guide SCRUM-175 through SCRUM-180.

Backend services, API responses, frontend integration, and automated tests must follow the documented contract.

## 13. Implementation Status

**Status:** API contract documented; backend implementation pending.

The repository investigation did not identify existing playlist controllers, services, entities, or API routes at the time of review.

The MySQL database schema already includes the required playlist storage tables.

The React frontend contains playlist components, but their backend API integration has not been verified.

Implementation will proceed through SCRUM-175 to SCRUM-181.

The contract defines the expected API behavior and provides the foundation for backend development.

Playlist endpoints must not be considered implemented or demo-ready until their functionality, database persistence, frontend integration, and automated tests have been verified.

Formal team review and Jira acceptance verification remain outstanding.