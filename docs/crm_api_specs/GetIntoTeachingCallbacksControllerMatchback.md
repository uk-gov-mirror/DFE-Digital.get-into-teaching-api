## POST `/api/get_into_teaching/callbacks/matchback`

Please check existing code and swagger doc for reference. There might be mistakes or things that I've missed here.
https://getintoteachingapi-test.test.teacherservices.cloud/swagger/index.html

**File:** `Controllers/GetIntoTeaching/CallbacksController.cs:90`

Matches a candidate by email in CRM and returns a pre-populated `GetIntoTeachingCallback` with their known data (name, phone number, candidate ID). This is a live request to the CRM.

## Request

```json
{
  "email": "candidate@example.com",
  "dateOfBirth": "1995-06-15",
  "firstName": "Jane",
  "lastName": "Doe",
  "reference": "ref"
}
```

| Param | Type | Required | Notes |
|-------|------|----------|-------|
| `email` | `string` | **Yes** | |
| `dateOfBirth` | `DateTime` | **Yes** | |
| `firstName` | `string` | No | |
| `lastName` | `string` | No | |
| `reference` | `string` | No | |

## Responses

### `200 OK` — candidate matched

Returns a full `GetIntoTeachingCallback` JSON. Only 5 fields are populated from CRM (`PopulateWithCandidate()`); the rest are null or computed defaults.

```json
{
  "candidateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "acceptedPolicyId": null,
  "email": "candidate@example.com",
  "firstName": "Jane",
  "lastName": "Doe",
  "addressTelephone": "123456789",
  "phoneCallScheduledAt": null,
  "talkingPoints": null,
  "creationChannelSourceId": null,
  "creationChannelServiceId": null,
  "creationChannelActivityId": null,
  "defaultContactCreationChannel": 222750043,
  "defaultCreationChannelSourceId": 222750003,
  "defaultCreationChannelServiceId": 222750007,
  "defaultCreationChannelActivityId": null
}
```

### `404 Not Found` — candidate not found. This is a new proposed error format

```json
{
    "errors": [
        {
            "error": "NotFound",
            "message": "Candidate with #{email} not found"
        }
    ]
}
```
