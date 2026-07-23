## GET `/api/mailing_list/members`

Finds an existing mailing_list candidate in CRM by email and date of birth, and returns `200`.

## Request

```json
{
  "email": "jane.doe@example.com",
  "dateOfBirth": "1995-06-15",
  "firstName": "Jane",
  "lastName": "Doe",
  "reference": "ref"
}
```

### Field details

| Param | Type | Required | Notes |
|-------|------|----------|-------|
| `email` | `string` | **Yes** | |
| `dateOfBirth` | `DateTime` | **Yes** | |
| `firstName` | `string` | No | |
| `lastName` | `string` | No | |
| `reference` | `string` | No | |

## Responses

### `200 OK` — candidate matched

Returns a pre-populated `MailingListAddMember`. Only the following fields are populated from CRM — all other fields (e.g. `graduationYear`, `citizenship`, `visaStatus`, `location`, `acceptedPolicyId`, `channelId`) will be `null`:

```json
{
  "candidateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "qualificationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "preferredTeachingSubjectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "considerationJourneyStageId": 222750001,
  "email": "jane.doe@example.com",
  "firstName": "Jane",
  "lastName": "Doe",
  "addressPostcode": "TE5 1IN",
  "welcomeGuideVariant": "England",
  "alreadySubscribedToEvents": false,
  "alreadySubscribedToMailingList": false,
  "alreadySubscribedToTeacherTrainingAdviser": false,
  "degreeStatusId": 222750000,
  "situation": null
}
```

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
