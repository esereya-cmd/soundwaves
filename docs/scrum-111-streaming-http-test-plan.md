# SCRUM-111 — Streaming HTTP Integration Test Plan

## Purpose
Verify the proposed Soundwaves Streaming API contract using HTTP integration tests.

## Endpoint
GET /api/tracks/{id}/stream

## Dependencies
- SCRUM-132: MP3 Streaming Endpoint
- SCRUM-133: HTTP Range Support
- SCRUM-134: Streaming Error Handling
- SCRUM-131: Secure Audio File Lookup Service (merged)

## Functional Test Cases

| ID | Scenario | Expected Result | Status |
|---|---|---|---|
| STR-01 | Existing track with available MP3 | 200, audio/mpeg | Planned |
| STR-02 | Valid Range request | 206, Content-Range | Planned |
| STR-03 | Invalid track ID (0) | 400 | Planned |
| STR-04 | Nonexistent track ID | 404 | Planned |
| STR-05 | Missing MP3 file | 404 | Planned |
| STR-06 | Unsatisfiable Range | 416 | Planned |

## Security Test Cases (SOC 2 CC6.6)

| ID | Scenario | Expected Result | Status |
|---|---|---|---|
| SEC-04 | Path traversal or unapproved file lookup | Access denied | Planned |
| SEC-06 | Unauthorized access, if required by policy | Request denied | Planned |
| SEC-07 | Invalid Range request | Safe response | Planned |
| SEC-08 | Internal information disclosure | No sensitive details | Planned |

## Execution Plan
1. Confirm the finalized API contract with the implementation owner.
2. Create isolated test database data and temporary MP3 fixtures.
3. Implement xUnit/WebApplicationFactory HTTP tests.
4. Run functional and negative tests after endpoint integration.
5. Record actual results and coordinate fixes with implementation owners.

## Current Limitation
The Streaming Endpoint has not been verified in the current main branch.
All cases remain Planned. No streaming test is reported as passed.

## Test Methodology Selection and Rationale

Test methodologies are selected before generating test cases.

### 1. Equivalence Partitioning
Divide track requests into valid identifiers, invalid identifiers,
nonexistent tracks, and tracks with missing audio resources.
Test representative values from each class.

### 2. Boundary Value Analysis
Test track ID boundaries, including zero and values outside the
supported ulong range. For supported HTTP Range requests,
test the first byte, final byte, and requests beyond file length.

### 3. Decision Table Testing
Evaluate combinations of:
- Track record exists or does not exist.
- Audio file exists or does not exist.
- HTTP request is valid or invalid.

Define the expected HTTP behavior for each relevant combination.

### 4. Negative and Security Testing
Test malformed inputs, inaccessible audio resources, invalid
Range headers, and attempted access outside the approved
media directory. Verify that error responses do not reveal
internal server information.

### 5. HTTP Contract and Integration Testing
Use xUnit and WebApplicationFactory to validate HTTP status
codes, Content-Type, Content-Length, Content-Range, response
bodies, and error behavior against the finalized SCRUM-130
streaming contract.

## Evidence and Traceability

For each executed test, record:
- Test case ID and methodology
- Related acceptance criterion
- Test inputs and expected outcome
- Actual result: Pass, Fail, Blocked, or Not Executed
- Relevant Git commit or PR
- Test execution output when available

Streaming tests must not be marked Passed until the endpoint
has been implemented and the tests have actually run.

## Decision Table — Streaming Request Handling

The following rules were derived from the SCRUM-130 proposed API
contract. Expected responses must be confirmed after implementation.

| Rule | Track Exists | Audio Available | Request Condition | Expected HTTP | Derived Test |
|---|---|---|---|---|---|
| DT-01 | Yes | Yes | Valid full-file request | 200 | STR-01 |
| DT-02 | No | N/A | Valid ID format | 404 | STR-04 |
| DT-03 | Yes | No | Valid full-file request | 404 | STR-05 |
| DT-04 | N/A | N/A | Track ID = 0 | 400 | STR-03 |
| DT-05 | Yes | Yes | Unsatisfiable Range | 416 | STR-06 |
| DT-06 | Yes | Yes | Valid partial Range | 206 | STR-02 |

N/A indicates a condition that is not applicable to the rule.

## Test Case Derivation and Traceability

| Test ID | Derived From | Related Criterion | Verification Target |
|---|---|---|---|
| STR-01 | DT-01, Equivalence Partitioning | SCRUM-111 | 200, audio/mpeg, audio bytes |
| STR-02 | DT-06, Boundary Value Analysis | AC-05 | 206, Content-Range, partial bytes |
| STR-03 | DT-04, Boundary Value Analysis | AC-01 | 400 for ID zero |
| STR-04 | DT-02, Equivalence Partitioning | AC-01 | 404 for nonexistent track |
| STR-05 | DT-03, Equivalence Partitioning | AC-04 | 404 for unavailable audio |
| STR-06 | DT-05, Boundary Value Analysis | AC-05 | 416 for unsatisfiable Range |
| SEC-04 | Negative Security Testing | AC-02 | No access outside media directory |
| SEC-06 | Access Policy Verification | AC-03 | Approved playback access policy |
| SEC-07 | Negative and Boundary Testing | AC-05 | Safe invalid Range handling |
| SEC-08 | Information Disclosure Testing | AC-04 | No sensitive information in errors |

## Current Assessment Status

All streaming-specific test cases remain Planned.

The secure audio lookup service has been integrated, but the
HTTP streaming endpoint has not been verified in the current
main branch. No streaming HTTP test is claimed as passed.
