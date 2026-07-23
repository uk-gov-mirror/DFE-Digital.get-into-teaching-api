## POST `/api/schools_experience/candidates/{id}/school_experience`

Please check existing code and swagger doc for reference. There might be mistakes or things that I've missed here.
https://getintoteachingapi-test.test.teacherservices.cloud/swagger/index.html

**File:** `Controllers/SchoolsExperience/CandidatesController.cs:154`

Adds a school experience record to an existing candidate. Returns `204 No Content`

## Request

```json
{
  "schoolUrn": "12345678",
  "durationOfPlacementInDays": 5,
  "dateOfSchoolExperience": "2026-09-15T00:00:00Z",
  "teachingSubjectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "notes": "The candidate showed great enthusiasm during their placement.",
  "schoolName": "James Brindley High School",
  "status": 1
}
```

### Route parameter

| Param | Type | Required | Notes |
|-------|------|----------|-------|
| `id` | `Guid` | **Yes** | The `candidateId` of the existing candidate |

### Body fields

| Param | Type | Required | Max length | Notes |
|-------|------|----------|------------|-------|
| `schoolUrn` | `string` | **Yes** | 8 | School URN (unique reference number) |
| `durationOfPlacementInDays` | `int` | No | ≤ 100 | Length of placement in days |
| `dateOfSchoolExperience` | `DateTime` | No | | Date of the school experience |
| `teachingSubjectId` | `Guid` | No | | Must be a valid teaching subject ID (validated against store) |
| `notes` | `string` | No | 2000 | Free-text notes about the experience |
| `schoolName` | `string` | No | 100 | Name of the school |
| `status` | `int` | No | | Defaults to `Requested` (1). See status enum below |

### School experience status

| Value | Name | Notes |
|-------|------|-------|
| `1` | `Requested` | Default status — set by CRM if not provided |
| `222750000` | `Confirmed` | |
| `222750001` | `DidNotAttend` | |
| `222750002` | `Rejected` | |
| `222750003` | `CancelledBySchool` | |
| `222750004` | `CancelledByCandidate` | |
| `222750005` | `Completed` | |
| `222750006` | `Withdrawn` | |

## Responses

### `204 No Content` — school experience queued for upsert

No body.

### `400 Bad Request` — invalid email. New proposed error format

```json
{
    "errors": [
        {
            "error": "BadRequest",
            "message": "Teaching Subject Id must be a valid teaching subject id."
        }
    ]
}
```
