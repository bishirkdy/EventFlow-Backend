# Event creation + event images

This backend now creates an event and its images with a single `multipart/form-data` request.

## 1. Apply the database migration

From the repository root:

```bash
dotnet ef database update \
  --project src/Services/Event/EventFlow.Event.Infrastructure \
  --startup-project src/Services/Event/EventFlow.Event.Api
```

This adds the `EventImages` table and the `EventImages.EventId -> Events.Id` foreign key.

## 2. Run the Event API

The development HTTPS profile uses:

```text
https://localhost:7002
```

Swagger:

```text
https://localhost:7002/swagger
```

## 3. Authorize in Swagger

Click **Authorize** and provide:

```text
Bearer <access-token>
```

The JWT handler also supports the existing `accessToken` HttpOnly cookie.

## 4. Create an event with images

Use:

```http
POST /api/v1/events/create
Content-Type: multipart/form-data
```

Form fields:

```text
Name
Description
EventType
SubType
StartDate
EndDate
TimeZone
Images[]
```

The backend accepts up to 10 images, with a maximum of 10 MB per image. Supported formats are JPEG, PNG, WebP, and GIF. The file signature is also checked instead of trusting only the MIME type.

Example timezone:

```text
Asia/Kolkata
```

## 5. Response

The 201 response returns the created event and the persisted image metadata:

```json
{
  "success": true,
  "message": "Event created successfully.",
  "data": {
    "id": "event-id",
    "name": "My Event",
    "images": [
      {
        "id": "image-id",
        "url": "/uploads/events/event-id/image.jpg",
        "originalFileName": "poster.jpg",
        "contentType": "image/jpeg",
        "sizeBytes": 123456,
        "displayOrder": 0
      }
    ]
  }
}
```

## 6. Verify the stored event

Call:

```http
GET /api/v1/events/{eventId}
```

The response includes the event and its images ordered by `displayOrder`.

The local development files are stored under the configured `FileStorage:RootPath`, which defaults to `wwwroot`, and are served through `/uploads/...`.


## Time zone note
The event API accepts IANA time zone IDs such as `Asia/Kolkata`. The backend uses `TimeZoneConverter` so IANA and Windows IDs work consistently on Windows and Linux. This avoids Windows/NLS failures from calling `TimeZoneInfo.FindSystemTimeZoneById` directly with an IANA ID.
