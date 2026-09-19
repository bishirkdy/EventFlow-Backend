# Cloudinary setup

The Event service uses Cloudinary through the BuildingBlocks storage abstraction.

## 1. Configure secrets

Use Visual Studio **Manage User Secrets** for `EventFlow.Event.Api` and add:

```json
{
  "Cloudinary": {
    "CloudName": "YOUR_CLOUD_NAME",
    "ApiKey": "YOUR_API_KEY",
    "ApiSecret": "YOUR_API_SECRET"
  }
}
```

Do not commit the API secret to `appsettings.json` or source control.

## 2. Event image storage

`EventImage.Url` stores the Cloudinary HTTPS delivery URL.
`EventImage.StorageKey` stores the Cloudinary public ID used for deletion.

Images are uploaded under:

`events/{eventId}`

## 3. Database

Cloudinary integration does not require a new EF migration. The existing
`EventImage` migrations remain unchanged.

## 4. Test

Create an event from Swagger with one or more images. A successful response
should contain an image URL beginning with `https://res.cloudinary.com/`.
