## GET `/api/teacher_training_adviser/candidates`

Finds an existing teacher_training_adviser candidate in CRM by email and date of birth, and returns `200`.

## Request

```json
{
  "email": "candidate@example.com",
  "dateOfBirth": "1995-06-15",
  "firstName": "Jane",
  "lastName": "Doe",
  "reference": "TTA"
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

Returns a full `TeacherTrainingAdviserSignUp` JSON with the candidate's existing CRM data.

```json
{
    "candidateId": "3a7e8f9c-...",
    "qualificationId": "b1c2d3e4-...",
    "subjectTaughtId": null,
    "pastTeachingPositionId": null,
    "preferredTeachingSubjectId": null,
    "countryId": "a1b2c3d4-...",
    "degreeCountry": null,
    "typeId": 222750001,
    "ukDegreeGradeId": null,
    "degreeTypeId": null,
    "degreeStatusId": 222750000,
    "initialTeacherTrainingYearId": null,
    "stageTaughtId": null,
    "preferredEducationPhaseId": null,
    "hasGcseMathsAndEnglishId": null,
    "hasGcseScienceId": null,
    "planningToRetakeGcseMathsAndEnglishId": null,
    "planningToRetakeGcseScienceId": null,
    "adviserStatusId": null,
    "email": "jane.doe@example.com",
    "firstName": "Jane",
    "lastName": "Doe",
    "dateOfBirth": "1995-06-15",
    "teacherId": null,
    "degreeSubject": "Physics",
    "addressTelephone": null,
    "addressPostcode": null,
    "graduationYear": 2023,
    "inferredGraduationDate": "2023-08-31",
    "canSubscribeToTeacherTrainingAdviser": false,
    "assignmentStatusId": 222750000
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
