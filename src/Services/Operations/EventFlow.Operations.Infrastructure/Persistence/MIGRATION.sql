-- Run once against OperationsDatabase before starting Operations API.
create table if not exists "AttendanceStaffAssignments" (
  "Id" uuid primary key,
  "EventId" uuid not null,
  "UserId" uuid not null,
  "ScopeType" integer not null,
  "ScopeId" uuid null,
  "IsActive" boolean not null default true,
  "CreatedAtUtc" timestamptz not null,
  "RevokedAtUtc" timestamptz null
);
create unique index if not exists "IX_AttendanceStaffAssignments_Unique" on "AttendanceStaffAssignments" ("EventId","UserId","ScopeType","ScopeId","IsActive");
create table if not exists "AttendanceRecords" (
  "Id" uuid primary key,
  "EventId" uuid not null,
  "RegistrationId" uuid not null,
  "ParticipantId" uuid not null,
  "ParticipantUserId" uuid not null,
  "SectionId" uuid null,
  "SessionId" uuid null,
  "StaffUserId" uuid not null,
  "Method" integer not null,
  "CheckedInAtUtc" timestamptz not null,
  "CheckedOutAtUtc" timestamptz null,
  "CreatedAtUtc" timestamptz not null
);
create unique index if not exists "IX_AttendanceRecords_Unique" on "AttendanceRecords" ("EventId","RegistrationId","SessionId");
create table if not exists "Notifications" (
  "Id" uuid primary key,
  "EventId" uuid not null,
  "UserId" uuid null,
  "RecipientEmail" varchar(320) not null,
  "Subject" varchar(300) not null,
  "Body" text not null,
  "Status" integer not null,
  "AttemptCount" integer not null,
  "ScheduledAtUtc" timestamptz not null,
  "SentAtUtc" timestamptz null,
  "FailedAtUtc" timestamptz null,
  "Error" text null,
  "CreatedAtUtc" timestamptz not null
);
create index if not exists "IX_Notifications_Status_Scheduled" on "Notifications" ("Status","ScheduledAtUtc");

-- If AttendanceRecords already exists from an earlier version:
alter table "AttendanceRecords" add column if not exists "ParticipantUserId" uuid not null default '00000000-0000-0000-0000-000000000000';

alter table "AttendanceRecords" add column if not exists "SectionId" uuid null;
