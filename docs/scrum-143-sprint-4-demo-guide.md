
# SCRUM-143 — Sprint 4 Demo Guide

**Project:** Soundwaves  
**Role:** Database / QA / CI/CD Support  
**Sprint:** 4

## 1. Demo Goal

Demonstrate that Soundwaves can retrieve music from MySQL, display catalog information, stream actual MP3 files, and support playback controls and error handling.

## 2. Test Environment

- Frontend: React / Vite
- Backend: ASP.NET Core
- Database: MySQL 8.4 through Docker
- Browser: Microsoft Edge
- Frontend URL: http://localhost:5173
- Backend URL: http://localhost:5072

## 3. Starting the Application

From the Soundwaves repository:

### Database
Start Docker Desktop, then run:

```powershell
cd backend-database
docker compose up -d
```

### Backend
From the repository root, run:

```powershell
dotnet run --project backend/Soundwaves.Api --launch-profile http
```

### Frontend
From the repository root, run:

```powershell
cd frontend
npm.cmd run dev
```

Confirm that the frontend, backend, and MySQL database are running before beginning the demo.

## 4. Demo Tracks

The following tracks were verified in my local development environment:

| ID | Track | Database File Path |
|---|---|---|
| 1 | Demo Track | audio/demo-track.mp3 |
| 3 | Demo Tone A | demo-tone-a.mp3 |

Both files were successfully streamed and played through the integrated application.

**Setup dependency:** These track records and file paths were verified in my local MySQL database. A fresh environment may require adding the MP3 fixtures and configuring their database records before reproducing playback.

## 5. Demo Walkthrough

1. Open Soundwaves at http://localhost:5173.
2. Show the catalog displaying tracks, albums, and artists.
3. Search for "demo" and verify matching results.
4. Open Demo Track and play the audio.
5. Return to the catalog and open Demo Tone A.
6. Play Demo Tone A and verify that track switching works.
7. Pause and resume playback.
8. Seek forward and backward.
9. Adjust volume and test mute/unmute.
10. Show the error response for a nonexistent track using `/api/tracks/999999/stream`.
11. Return to Soundwaves and verify that normal playback still works.

## 6. Sprint 4 Test Results

| Jira Ticket | Verification |
|---|---|
| SCRUM-138 | Catalog-to-stream integration verified |
| SCRUM-139 | Real MP3 playback, switching, and search-to-play passed |
| SCRUM-140 | Seek, volume, pause, resume, and mute tests passed |
| SCRUM-141 | Missing-file errors and recovery verified |
| SCRUM-142 | Catalog, search, and details regression verified |

## 7. Issues Found During Testing

- Streaming initially failed because the backend route had not yet been integrated. After the streaming endpoint was merged, the feature was retested successfully.
- Demo Track initially had an incompatible absolute-style database path. Correcting the local record allowed playback.
- Demo Tone A initially returned HTTP 404 because its database path did not match the actual file location. I corrected the local path and successfully retested.
- These local database corrections have not been added to a reproducible seed-data setup.

## 8. Evidence

Evidence was recorded in the corresponding Jira tickets, including:

- Catalog and search screenshots
- Successful MP3 playback
- Browser Network responses
- Database path verification
- Missing-file and invalid-ID responses
- Successful playback retests

## 9. Demo Readiness

The tested features are working in my local development environment.

Before demonstrating on another machine, the team should verify that the audio fixtures, MySQL track records, backend configuration, and required services are available.

**Local Test Result:** PASS
