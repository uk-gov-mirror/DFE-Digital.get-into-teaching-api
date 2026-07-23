## POST `/api/get_into_teaching/callbacks`

Please check existing code and swagger doc for reference. There might be mistakes or things that I've missed here.
https://getintoteachingapi-test.test.teacherservices.cloud/swagger/index.html

**File:** `Controllers/GetIntoTeaching/CallbacksController.cs:38`

Schedules a callback (phone call) for a candidate. Validates the request, builds a `Candidate` with minimal business logic (phone call + privacy policy), serializes with change tracking, and enqueues an `UpsertCandidateJob` to persist to CRM. Returns `204 No Content` — CRM upsert is async.

## Request

```json
{
  "candidateId": null,
  "email": "jane.doe@example.com",
  "firstName": "Jane",
  "lastName": "Doe",
  "addressTelephone": "07123456789",
  "phoneCallScheduledAt": "2026-06-20T14:30:00Z",
  "talkingPoints": "I want to know more about teaching Maths",
  "acceptedPolicyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Field details

| Param | Type | Required | Notes |
|-------|------|----------|-------|
| `email` | `string` | **Yes** | Validated for format + max 100 chars |
| `firstName` | `string` | **Yes** | Non-empty |
| `lastName` | `string` | **Yes** | Non-empty |
| `addressTelephone` | `string` | **Yes** | Validated 5-25 chars, no alpha chars |
| `phoneCallScheduledAt` | `DateTime` | **Yes** | Must be in the future |
| `talkingPoints` | `string` | **Yes** | Non-empty — what the candidate wants to discuss |
| `acceptedPolicyId` | `Guid` | **Yes** | |
| `candidateId` | `Guid` | No | Set for existing candidates (matchback/exchange) — null for new callbacks |
| `creationChannelSourceId` | `int` | No | Overrides default GIT Website source |
| `creationChannelServiceId` | `int` | No | Overrides default Mailing List service |
| `creationChannelActivityId` | `int` | No | Overrides default null activity |

## Responses

### `204 No Content` — callback queued for upsert

No body.

### `400 Bad Request` — validation failed. This is a new proposed error format

```json
{
    "errors": [
        {
            "error": "BadRequest",
            "message": "Email is not a valid email address"
        }
    ]
}
```
