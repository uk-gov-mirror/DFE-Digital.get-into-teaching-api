## GET `/api/schools_experience/candidates

Finds an existing schools_experience candidate in CRM by email and date of birth, and returns `200`.

## Request

```json
{
  "email": "jane.doe@example.com",
  "dateOfBirth": "1990-01-15",
  "firstName": "Jane",
  "lastName": "Doe",
  "reference": "ref"
}
```

### Body fields

| Param | Type | Required | Notes |
|-------|------|----------|-------|
| `email` | `string` | **Yes** | |
| `dateOfBirth` | `date` | **Yes** | |
| `firstName` | `string` | No | |
| `lastName` | `string` | No | |
| `reference` | `string` | No | |

## Responses

### `200 OK` — candidate matched and authenticated

Body is a pre-populated `SchoolsExperienceSignUp` with fields filled from the matched candidate record:

```json
{
  "candidateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "preferredTeachingSubjectId": null,
  "secondaryPreferredTeachingSubjectId": null,
  "masterId": null,
  "merged": false,
  "fullName": "Jane Doe",
  "email": "jane.doe@example.com",
  "firstName": "Jane",
  "lastName": "Doe",
  "addressLine1": "10 Downing Street",
  "addressLine2": null,
  "addressLine3": null,
  "addressCity": "London",
  "addressStateOrProvince": "London",
  "addressPostcode": "SW1A 1AA",
  "telephone": "07123456789",
  "hasDbsCertificate": true,
  "dbsCertificateIssuedAt": "2024-01-15T00:00:00Z",
  "qualificationId": null,
  "degreeStatusId": null,
  "degreeTypeId": null,
  "degreeSubject": null,
  "ukDegreeGradeId": null,
  "defaultContactCreationChannel": 222750021,
  "defaultCreationChannelSourceId": 222750013,
  "defaultCreationChannelServiceId": 222750001,
  "defaultCreationChannelActivityId": null,
  "creationChannelSourceId": null,
  "creationChannelServiceId": null,
  "creationChannelActivityId": null
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
