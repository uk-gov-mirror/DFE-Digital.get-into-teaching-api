## POST `/api/teacher_training_adviser/candidates/matchback`

Please check existing code and swagger doc for reference. I might have made mistakes or missed something here.
https://getintoteachingapi-test.test.teacherservices.cloud/swagger/index.html

**File:** `Controllers/TeacherTrainingAdviser/CandidatesController.cs:117`

Matches a candidate by email in CRM and returns a pre-populated `TeacherTrainingAdviserSignUp` with their known data (name, DOB, qualifications, past teaching positions, etc.)

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
| `firstName` | `string` | No |  |
| `lastName` | `string` | No |  |
| `reference` | `string` | No | |

## Responses

### `200 OK` — candidate matched

Returns a pre-populated `TeacherTrainingAdviserSignUp`. Key fields populated from CRM:

```json
{
  "candidateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "candidate@example.com",
  "dateOfBirth": "1995-06-15",
  "teacherId": "1234567",
  "addressTelephone": "07123456789",
  "addressPostcode": "TE5 1IN",
  "typeId": 222750000,
  "adviserStatusId": null,
  "assignmentStatusId": null,
  "canSubscribeToTeacherTrainingAdviser": true,
  "qualificationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "degreeSubject": "Mathematics",
  "ukDegreeGradeId": 222750000,
  "degreeTypeId": 222750000,
  "degreeStatusId": 222750000,
  "pastTeachingPositionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "subjectTaughtId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "preferredTeachingSubjectId": null,
  "countryId": null,
  "initialTeacherTrainingYearId": null,
  "preferredEducationPhaseId": null,
  "hasGcseMathsAndEnglishId": null,
  "hasGcseScienceId": null,
  "planningToRetakeGcseMathsAndEnglishId": null,
  "planningToRetakeGcseScienceId": null
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
