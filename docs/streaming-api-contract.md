# Soundwaves â€” Streaming API Contract

**Jira:** SCRUM-130 â€” Define Streaming API Route and Response Contract
**Sprint:** 4
**Component:** ASP.NET Core Backend
**Status:** Proposed â€” Pending Team Review

## 1. Purpose

Define a consistent and secure HTTP API contract for streaming MP3 audio from the Soundwaves backend to the frontend music player.

This contract establishes the endpoint, expected responses, HTTP headers, error handling, and security requirements before implementation.

## 2. Streaming Endpoint

**Method:** GET

**Route:** `/api/tracks/{id}/stream`

**Path Parameter:**

- `id`: Unsigned 64-bit track identifier (`ulong`).

The frontend requests audio using a track ID rather than providing a filesystem path.

Example request:

```http
GET /api/tracks/1/stream HTTP/1.1
```

The backend retrieves the track record, resolves the associated audio file, and returns the audio content.

## 3. Successful Streaming Response

A successful full-file request returns:

```http
HTTP/1.1 200 OK
Content-Type: audio/mpeg
Content-Length: <file-size-in-bytes>
Accept-Ranges: bytes
```

The response body contains the MP3 audio data.

The backend should stream the file rather than load the entire audio file into memory.

## 4. HTTP Byte-Range Support

The endpoint must support HTTP byte-range requests to allow seeking during playback.

Example request:

```http
GET /api/tracks/1/stream HTTP/1.1
Range: bytes=0-1023
```

Expected response:

```http
HTTP/1.1 206 Partial Content
Content-Type: audio/mpeg
Accept-Ranges: bytes
Content-Range: bytes 0-1023/<total-file-size>
Content-Length: 1024
```

The response body contains only the requested bytes.

Supported scenarios should include:

- Requests without a Range header.
- Valid single-byte ranges.
- Open-ended ranges.
- Suffix-byte ranges.
- Unsatisfiable ranges.

ASP.NET Core's built-in file response functionality should be used where appropriate.

## 5. Error Handling

| Scenario | Expected Behavior |
|---|---|
| Track ID is zero | 400 Bad Request |
| Track ID cannot be parsed | 400 Bad Request |
| Track does not exist | 404 Not Found |
| Audio file does not exist | 404 Not Found |
| Audio file is inaccessible | Controlled failure without exposing internal paths |
| File path is outside the approved directory | Reject access without exposing filesystem details |
| Range is unsatisfiable | 416 Range Not Satisfiable |
| Malformed Range header | Document and test framework-specific behavior |

For an unsatisfiable range, return the appropriate `Content-Range: bytes */<total-file-size>` header.

Error responses must not reveal absolute filesystem paths, internal exceptions, or sensitive server information.

## 6. Audio File Resolution and Security

The existing `Track` model contains:

- `Id`
- `FilePath`
- `MimeType`
- `FileSizeBytes`
- `DurationMs`

The current sample catalog references `/music/sample-track.mp3`.

The physical media directory has not yet been configured, and no MP3 files were found in the inspected repository.

The implementation must:

1. Resolve the track using its database identifier.
2. Obtain the associated stored audio path.
3. Resolve that path against an approved, configurable media directory.
4. Normalize and validate the resolved path.
5. Reject paths outside the approved directory, including path traversal and filesystem-link escapes.
6. Avoid accepting arbitrary client-provided filesystem paths.
7. Return controlled errors when media is missing or inaccessible.

The logical `/music/` path convention must be mapped to an approved physical directory during SCRUM-131.

## 7. Frontend Integration

The frontend player will request audio through:

`/api/tracks/{id}/stream`

The endpoint must support browser audio playback and seeking.

The frontend should not require direct access to the backend filesystem.

Frontend integration and end-to-end playback verification are assigned to the appropriate team members.

## 8. Testing Strategy

The following Jira tasks will validate the streaming implementation:

- SCRUM-131 â€” Secure Audio File Lookup Service
- SCRUM-132 â€” MP3 Streaming Endpoint
- SCRUM-133 â€” HTTP Range and Partial Response Support
- SCRUM-134 â€” Streaming Error Behavior
- SCRUM-135 â€” Streaming Service Unit Tests
- SCRUM-136 â€” Backend Build and Streaming Tests in CI

Automated backend tests will cover successful lookup, missing resources, invalid paths, and relevant streaming response behavior.

Manual testing will use a real, permitted MP3 file to verify successful audio delivery and byte-range responses.

System-level playback and user interaction testing will be coordinated with the end-to-end testing owner.

## 9. Acceptance Criteria

- [ ] The canonical streaming route is documented.
- [ ] Request parameters and successful responses are defined.
- [ ] HTTP 200, 206, 400, 404, and 416 behaviors are documented.
- [ ] Required response headers are defined.
- [ ] Secure media lookup requirements are documented.
- [ ] Frontend and API-testing owners review the contract.
- [ ] The contract is linked to SCRUM-130.
- [ ] The proposed endpoint is not represented as already implemented.

## 10. Implementation Status

This document defines the proposed streaming API contract.

The endpoint, secure file lookup, range handling, and automated tests will be implemented in subsequent Sprint 4 tasks.

Any contract changes must be coordinated with the frontend and API-testing owners before integration.
