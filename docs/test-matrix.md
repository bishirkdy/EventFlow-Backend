# EventFlow Test Matrix

Source: `test.txt` (flows 02-19) x implemented roles: `participant`, `photographer`, `organizer`, `owner`, `attendance-staff`.

Status legend: **PASS** verified working / **FIXED** gap repaired and verified in this release (text after arrow names the fix).

## Role capability matrix

| Capability | participant | photographer | organizer | owner | attendance-staff |
|---|---|---|---|---|---|
| Browse public event site | yes (public) | yes | yes | yes | yes |
| Register / my registrations / ticket QR | yes | yes (as user) | yes | yes | yes |
| Create event, manage schedule/registration/certificates | no | no | yes (`event.update`) | inherited | no |
| Invite/revoke team, assign staff | no | no | yes (`event.team.manage`) | yes (`event.team.manage`) | no |
| Claim ownership | creator only | no | creator only | yes (auto-claim) | no |
| Analytics dashboards | no | no | yes (overview) | yes (read-only) | no |
| Upload photos | no | yes (`photo.upload`) | no (moderation only) | no | no |
| Moderate (approve/hide/delete) photos | no | no | yes (`photo.update`) | no | no |
| Assign/revoke attendance staff | no | no | yes (`event.team.manage`) | yes | no |
| Check-in/out (scoped) | no | no | yes (event scope) | no | yes (event/section/session scope) |
| View own attendance history | yes (self) | yes (self) | yes (event) | yes (event) | yes (self) |
| Submit feedback | yes | yes | yes | yes | yes |
| View feedback results | no | no | yes | yes | no |
| View certificates / verify | yes (own) | yes (own) | yes | yes | public verify |

## Flow matrix

| # | Flow | Backend | Frontend | Status |
|---|---|---|---|---|
| 02 | Participant registration + participant creation + status | `Registration/.../RegistrationsController`, `ParticipantsController`, `CreateRegistrationCommandHandler` | `platform/registration/pages/public/register`, `registration-form` | PASS |
| 03 | Organizer registration management (list/search/filter/details) | `RegistrationsController` List/Get, `GetRegistrationAnalytics` | `organizer/pages/registration/{registrations,participants,registration-details}` | PASS |
| 04 | Approve/reject + status + reason + ticket trigger | `ApproveRegistration`, `RejectRegistration` handlers (capacity-checked) | `registrations.ts`, `registration-details.ts` | PASS |
| 05 | Cancellation + ticket revocation + capacity release | `CancelRegistration` handler; counts filter `Approved` only | `my-registrations.ts` | FIXED: auto-promote on cancel/reject |
| 06 | Capacity + availability + full handling | `Create/Approve/Promote` capacity checks (`CapacityMode`, `Capacity`) | `registration-form.ts` capacity settings | PASS |
| 07 | Waitlist queue + position + promotion | `WaitlistRegistration`, `PromoteWaitlistedRegistration` | `registration.service.ts` waitlist/promote | FIXED: auto-promote on cancel/reject |
| 08 | Participant portal (events, registration, ticket, schedule, attendance, photos, certificate, feedback) | composed across services | `platform/my-events`, `attendance-history`, `photos/*`, `certificates/my-certificate`, `website.ts` | FIXED: feedback module (submit + results) |
| 09 | Ticket + secure QR + verification | `TicketsController`, `verify-internal` -> `Operations AttendanceService` | `ticket.ts`, `qr-code.ts` | PASS |
| 10 | Sections + sessions + venues + speakers + conflict | `SectionController`, `SessionController`, `VenueController`, `SpeakerController`; conflict in `Create/UpdateSessionCommandHandler` | `organizer/pages/{sections,sessions,venues,speakers}/*` | FIXED: speaker conflict check (assign + reschedule) |
| 11 | Scoped attendance staff + dashboard | `AttendanceStaffController`, `AttendanceStaffService` | `organizer/pages/attendance-staff` + staff routes | FIXED: staff routes + guard + scope UI |
| 12 | Event check-in/out QR + verification | `AttendanceController` `checkInQr`/`checkInManual`/`checkOut`, `CanOperateAsync` | `attendance-staff/pages/scanner`, `organizer/pages/attendance` | FIXED: send scope from scanner |
| 13 | Session attendance + participation tracking | `AttendanceController` with `sectionId/sessionId` | scanner + attendance pages | FIXED: scope picker |
| 14 | Attendance history + % | `GetAttendanceHistory`, `GetAttendanceAnalytics` | `platform/attendance-history` | PASS |
| 15 | Notifications + targeted + email + background | `NotificationsController`, `NotificationWorker`, `IEmailSender` | `organizer/pages/notifications` | FIXED: worker drain + MailKit SMTP |
| 16 | Photographer upload + media + Cloudinary | `EventPhotoController`, `PhotographerInvitationController` | `photographer/dashboard/*`, `photo-moderation-grid`, gallery | FIXED: multipart upload + `photo.*` |
| 17 | Certificates rules + eligibility + generation + download | `CertificatesController`, `CertificateVerifyController` | `my-certificate`, `certificate-verify`, `certificate-settings` | PASS |
| 18 | Feedback event/session/speaker/venue + organizer results | `Features/Feedback/*` (Event service) | participant submit + organizer results pages | FIXED: feedback module (submit + results) |
| 19 | Organizer analytics | `EventAnalyticsController`, `RegistrationAnalyticsController`, `CertificateAnalyticsController`, `AttendanceAnalyticsController`, `EventTeamAnalyticsController` | `owner-dashboard`, `organizer/pages/overview` | PASS |

## Gateway routes (YARP)

Analytics endpoints resolve as follows (all through `/api/v1/...` on gateway `:5000`):

| Endpoint | Route key | Order |
|---|---|---|
| `events/{id}/analytics/{overview,programme,content}` | `event-route` | 0 |
| `events/{id}/registrations/analytics` | `registration-route` | -1 |
| `events/{id}/certificates/analytics` | `certificate-route` | -1 |
| `operations/events/{id}/attendance/analytics` | `operations-route` | 0 |
| `events/{id}/team/analytics` | `identity-event-team-route` | -1 (fixed) |
