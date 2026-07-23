## GET `/api/candidates`

Finds an existing candidate in CRM by email, and returns `200`.

- Live CRM query: searches `emailaddress1`/`emailaddress2` for equivalent email variants (gmail.com ↔ googlemail.com), active candidates only
- Also searches by the additional request params
- If candidate found — returns `200`
- If no candidate found — returns `404`

## Request

```json
{
  "email": "candidate@example.com",
  "firstName": "Jane",
  "lastName": "Doe",
  "dateOfBirth": "1995-06-15",
  "reference": "TTA"
}
```

| Param | Type | Required |
|-------|------|----------|
| `email` | `string` | **Yes** |
| `firstName` | `string` | No |
| `lastName` | `string` | No |
| `dateOfBirth` | `DateTime` | No |
| `reference` | `string` | No |

## Responses

### `200`

No body.

### `404 Not Found` — candidate not found. New proposed error format


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
