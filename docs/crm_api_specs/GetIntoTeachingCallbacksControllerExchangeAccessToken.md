## GET `/api/get_into_teaching/callbacks`

Finds an existing callback candidate in CRM by email and date of birth, and returns `200`.

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

### `200 OK` 

Returns a full `GetIntoTeachingCallback` JSON with the candidate's existing CRM data pre-populated (same response shape as matchback).

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

Only `CandidateId`, `Email`, `FirstName`, `LastName`, `AddressTelephone` are populated from CRM. All other fields are null or computed from `ICreateContactChannel` interface defaults.

### `404 NotFound` — candidate not found. New proposed error format

```json
{
    "errors": [
        {
            "error": "NotFound",
            "message": "Candidate not found"
        }
    ]
}
```
