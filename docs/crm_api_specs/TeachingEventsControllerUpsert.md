## POST `/api/teaching_events`

Please check existing code and swagger doc for reference. I might have made mistakes or missed something here.
https://getintoteachingapi-test.test.teacherservices.cloud/swagger/index.html

**File:** `Controllers/GetIntoTeaching/TeachingEventsController.cs:207`

Adds or updates a teaching event. If the payload includes an `id` the existing event is updated, otherwise a new one is created. Persists both the event and its venue (building) to Dynamics 365 CRM.

## Request

```json
{
  "typeId": 222750000,
  "statusId": 222750000,
  "readableId": "my-event-2024",
  "name": "My Event",
  "summary": "A summary of the event",
  "description": "A full description of the event",
  "startAt": "2024-06-01T09:00:00Z",
  "endAt": "2024-06-01T17:00:00Z",
  "isOnline": false,
  "registrationEmailLink": "registration@test.test",
  "building": {
    "venue": "Venue Name",
    "addressLine1": "123 Street",
    "addressCity": "London",
    "addressPostcode": "SW1A 1AA"
  }
}
```

### Key fields

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `typeId` | `int` | **Yes** | Event type (e.g. `ApplicationWorkshop = 222750000`, `TrainToTeachEvent = 222750001`) |
| `statusId` | `int` | **Yes** | Event status (e.g. `Open = 222750000`, `Closed = 222750001`, `Draft = 222750002`, `Pending = 222750003`) |
| `readableId` | `string` | **Yes** | URL-friendly unique identifier; must match regex `\A[^_\W][\w-]+[^_\W]\Z` and be unique across all events |
| `name` | `string` | **Yes** | Event name |
| `startAt` | `DateTime` | **Yes** | Event start time |
| `endAt` | `DateTime` | **Yes** | Event end time; must be >= `startAt` |
| `isOnline` | `bool` | No | Whether the event is online |
| `registrationEmailLink` | `string` | No | An email address of weblink for registering for the event |
| `building` | `TeachingEventBuilding` | No | Venue details; if provided, the building is persisted first and linked to the event |
| `webFeedId` | `string` | No | If set, the API will accept new attendees for this event (external sign-up used if nil) |
| `summary` | `string` | No | Short event summary |
| `description` | `string` | No | Full event description |
| `videoUrl` | `string` | No | Video link for online events |
| `providerWebsiteUrl` | `string` | No | Provider's website URL |
| `providerTargetAudience` | `string` | No | Target audience description |
| `providerOrganiser` | `string` | No | Organiser name |
| `providerContactEmail` | `string` | No | Contact email (validated for email format, max 100 chars) |
| `providersList` | `string` | No | Comma-separated list of providers |
| `regionId` | `int?` | No | Region option set value |
| `message` | `string` | No | Miscellaneous message (e.g. if event is nearly booked out) |
| `scribbleId` | `string` | No | Scribble ID |
| `accessibilityOptions` | `int[]` | No | Array of accessibility option set values |

### Building fields

| Field | Type | Notes |
|-------|------|-------|
| `venue` | `string` | Required; venue name |
| `addressLine1` | `string` | Address line 1 |
| `addressLine2` | `string` | Address line 2 |
| `addressLine3` | `string` | Address line 3 |
| `addressCity` | `string` | City |
| `addressPostcode` | `string` | Required; validated as a UK postcode |
| `imageUrl` | `string` | Event venue image URL |

## Responses

### `201 Created` — event created or updated

```json
{
  "id": "a1b2c3d4-...",
  "typeId": 222750000,
  "statusId": 222750000,
  "readableId": "my-event-2024",
  "referenceNumber": "A1234",
  "name": "My Event",
  "summary": "A summary of the event",
  "description": "A full description of the event",
  "message": null,
  "startAt": "2024-06-01T09:00:00Z",
  "endAt": "2024-06-01T17:00:00Z",
  "isOnline": false,
  "isVirtual": false,
  "isInPerson": true,
  "building": {
    "id": "b2c3d4e5-...",
    "venue": "Venue Name",
    "addressLine1": "123 Street",
    "addressLine2": null,
    "addressLine3": null,
    "addressCity": "London",
    "addressPostcode": "SW1A 1AA",
    "imageUrl": null
  },
  "webFeedId": null,
  "videoUrl": null,
  "providerWebsiteUrl": null,
  "providerTargetAudience": null,
  "providerOrganiser": null,
  "providerContactEmail": null,
  "providersList": null,
  "regionId": null,
  "scribbleId": null,
  "registrationEmailLink": "registration@test.test",
  "accessibilityOptions": []
}
```

### `400 Bad Request` — validation failed. New proposed error format

```json
{
    "errors": [
        {
            "error": "BadRequest",
            "message": "ReadableId does not match the required pattern",
            "attribute": "ReadableId"
        }
    ]
}
```
