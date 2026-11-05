# SCRUM-137 — Integrated Playback End-to-End Test Plan

**Project:** Soundwaves  
**Sprint:** 4  
**Role:** Database / QA / CI/CD Support  
**Status:** Draft — Pending Integration Readiness

## 1. Objective

Verify that a user can browse the Soundwaves catalog, select a track, stream the correct MP3 through the ASP.NET Core backend, and control playback from the React frontend.

The testing will cover both successful playback and error handling to make sure the application remains functional when something goes wrong.


## 2. Testing Environment

- Frontend: React
- Backend: ASP.NET Core
- Database: MySQL 8.4
- Development tools: VS Code and Docker Compose
- Test device: Windows development computer
- Test browser: Microsoft Edge
- Source branch: Latest integrated main
- Frontend entry point: Catalog → Select Song → MusicPlayer
- Expected backend endpoint: GET /api/tracks/{id}/stream
- Test data: Two approved MP3 files linked to database track IDs (pending confirmation)
- Startup instructions: Pending verification with the team
- Environment status: Not yet verified for full playback


## 3. Current Implementation Findings

Based on our inspection of the current project files:

- `App.jsx` passes selected tracks from the catalog to `MusicPlayer`.
- `MusicPlayer.jsx` displays the selected track but does not currently contain audio playback functionality.
- `AudioFileLookupService.cs` implements database-backed audio file lookup and checks for missing or unsafe paths.
- The intended endpoint is `GET /api/tracks/{id}/stream`.
- The HTTP streaming endpoint has not yet been verified in the inspected implementation.
- Existing streaming integration tests exercise the file lookup service rather than the actual streaming HTTP endpoint.

These are code inspection findings, not completed end-to-end test results.

## 4. Test Scenarios

| Test ID | Scenario | Expected Result | Status |
|---|---|---|---|
| E2E-01 | Select a known catalog track | Correct MP3 plays | Not Executed |
| E2E-02 | Search for a known track and select it | Correct track plays | Not Executed |
| E2E-03 | Switch between two tracks | New track plays and previous playback stops | Not Executed |
| E2E-04 | Pause and resume | Playback pauses and resumes correctly | Not Executed |
| E2E-05 | Seek and adjust volume | Controls respond correctly | Not Executed |
| E2E-06 | Request missing audio | Error is displayed and navigation remains available | Not Executed |
| E2E-07 | Request an invalid track ID | Request fails safely without crashing the application | Not Executed |

## 5. Test Execution Steps

1. Start the frontend, backend, and MySQL environment.
2. Verify two approved test tracks exist in the database.
3. Open the Soundwaves catalog.
4. Select the first track.
5. Inspect the browser Network tab for the streaming request.
6. Verify that the requested media corresponds to the selected track.
7. Confirm audible playback and player controls.
8. Repeat using a second track and the search feature.
9. Test missing files and invalid requests.
10. Record actual results and supporting evidence.

## 6. Evidence Collection

For each test, record:

- Test ID and associated Jira ticket
- Test inputs and reproduction steps
- Expected and actual results
- Pass, Fail, Blocked, or Not Executed
- Tested branch or commit
- Browser screenshots and Network tab results
- Relevant backend logs and Jira defects


## 7. Team Handoffs

Integration testing and defect reporting:
- Derrick: Execute cross-layer playback tests, collect evidence,
  document defects in Jira, and retest resolved issues.

Implementation and defect resolution:
- Frontend player owner: Investigate and fix React playback issues.
- Backend streaming owner: Investigate and fix API or streaming errors.
- Database owner: Assist with track records and media references.

Coordination:
- Eduardo: Confirm feature readiness, implementation ownership,
  and the agreed testing schedule.
- Derrick: Communicate test results and blockers to the team.

Handoff process:
Handoff agreement status: Partially confirmed.
Gogun has agreed to the proposed workflow.
Remaining details require confirmation from
Eduardo, Te Yen, and other relevant owners.

Handoff agreement status: Proposed; awaiting confirmation
from Eduardo and the relevant feature owners.

## 8. Current Dependencies

Full playback testing requires a working HTTP streaming endpoint, functional React player controls, real playable MP3 files, and an integrated development environment.

Until these requirements are confirmed, all end-to-end scenarios remain **Not Executed**. If a test cannot run because a required dependency is unavailable, mark it **Blocked** and record the reason.

**Next Action:** Confirm playback implementation readiness, testing environment, test assets, and team handoffs before executing E2E scenarios.