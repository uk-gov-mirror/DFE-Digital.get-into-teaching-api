## POST `/api/schools_experience/candidates`

Please check existing code and swagger doc for reference. There might be mistakes or things that I've missed here.
https://getintoteachingapi-test.test.teacherservices.cloud/swagger/index.html

**File:** `Controllers/SchoolsExperience/CandidatesController.cs:44`

Upserts a candidate for the Schools Experience service. Validates the request, builds a `Candidate`, and returns `201 Created` with the `SchoolsExperienceSignUp` response body.

## Request

```json
{
  "candidateId": null,
  "preferredTeachingSubjectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "secondaryPreferredTeachingSubjectId": null,
  "acceptedPolicyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
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
  "creationChannelSourceId": null,
  "creationChannelServiceId": null,
  "creationChannelActivityId": null
}
```

### Field details

| Param | Type | Required | WriteOnly | Notes |
|-------|------|----------|-----------|-------|
| `preferredTeachingSubjectId` | `Guid` | **Yes** | | |
| `secondaryPreferredTeachingSubjectId` | `Guid` | No | | |
| `acceptedPolicyId` | `Guid` | **Yes** | Yes | Write-only — not returned in response |
| `email` | `string` | **Yes** | | Non-empty |
| `firstName` | `string` | **Yes** | | Non-empty |
| `lastName` | `string` | **Yes** | | Non-empty |
| `addressLine1` | `string` | **Yes** | | Non-null |
| `addressLine2` | `string` | No | | |
| `addressLine3` | `string` | No | | |
| `addressCity` | `string` | **Yes** | | Non-null |
| `addressStateOrProvince` | `string` | **Yes** | | Non-null |
| `addressPostcode` | `string` | **Yes** | | Non-null; formatted via `.AsFormattedPostcode()` |
| `telephone` | `string` | **Yes** | | Non-null; mapped to `Candidate.AddressTelephone` |
| `hasDbsCertificate` | `bool` | **Yes** | | Non-null |
| `dbsCertificateIssuedAt` | `DateTime` | No | | |
| `qualificationId` | `Guid` | No | | Only stored if a qualification is being added |
| `degreeStatusId` | `int` | No | | |
| `degreeTypeId` | `int` | No | | Defaults to `Degree` (222750000) if qualification is created |
| `degreeSubject` | `string` | No | | |
| `ukDegreeGradeId` | `int` | No | | |
| `candidateId` | `Guid` | No | | Set for existing candidates (matchback/exchange) |
| `creationChannelSourceId` | `int` | No | Yes | Overrides default SchoolExperience (222750013) |
| `creationChannelServiceId` | `int` | No | Yes | Overrides default CreatedOnSchoolExperience (222750001) |
| `creationChannelActivityId` | `int` | No | Yes | Overrides default null |

### Response fields (read-only, returned in body)

| Param | Type | Notes |
|-------|------|-------|
| `candidateId` | `Guid` | The candidate's ID |
| `masterId` | `Guid` | Present if merged with another record |
| `merged` | `bool` | Whether the candidate has been merged |
| `fullName` | `string` | Computed full name |
| `email` | `string` | |
| `firstName` | `string` | |
| `lastName` | `string` | |
| `addressLine1` | `string` | |
| `addressLine2` | `string` | |
| `addressLine3` | `string` | |
| `addressCity` | `string` | |
| `addressStateOrProvince` | `string` | |
| `addressPostcode` | `string` | |
| `telephone` | `string` | Fallback: `AddressTelephone` → `Telephone` → `MobileTelephone` → `SecondaryTelephone` (all `.StripExitCode()`) |
| `hasDbsCertificate` | `bool` | |
| `dbsCertificateIssuedAt` | `DateTime` | |
| `preferredTeachingSubjectId` | `Guid` | |
| `secondaryPreferredTeachingSubjectId` | `Guid` | |
| `qualificationId` | `Guid` | From the candidate's latest qualification |
| `degreeStatusId` | `int` | From the candidate's latest qualification |
| `degreeTypeId` | `int` | From the candidate's latest qualification |
| `degreeSubject` | `string` | From the candidate's latest qualification |
| `ukDegreeGradeId` | `int` | From the candidate's latest qualification |
| `defaultContactCreationChannel` | `int` | Always 222750021 (SchoolsExperience) |
| `defaultCreationChannelSourceId` | `int` | Always 222750013 (SchoolExperience) |
| `defaultCreationChannelServiceId` | `int` | Always 222750001 (CreatedOnSchoolExperience) |
| `defaultCreationChannelActivityId` | `int?` | Always null |
| `creationChannelSourceId` | `int` | From the candidate's latest ContactChannelCreation |
| `creationChannelServiceId` | `int` | From the candidate's latest ContactChannelCreation |
| `creationChannelActivityId` | `int` | From the candidate's latest ContactChannelCreation |

## Responses

### `201 Created` — candidate upserted

```json
{
  "candidateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "preferredTeachingSubjectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
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
  "creationChannelSourceId": 222750013,
  "creationChannelServiceId": 222750001,
  "creationChannelActivityId": null
}
```

### `400 Bad Request` — validation failed. New proposed error format
```json
{
    "errors": [
        {
            "error": "BadRequest",
            "message": "Email must not be empty"
        }
    ]
}
```
