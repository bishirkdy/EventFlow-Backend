namespace EventFlow.Security.Authorization;

public static class PermissionConstants
{
    public static class Event
    {
        public const string Create = "event.create";
        public const string View = "event.view";
        public const string Update = "event.update";
        public const string Delete = "event.delete";
        public const string Manage = "event.manage";
        public const string TeamManage = "event.team.manage";
    }

    public static class Registration
    {
        public const string View = "registration.view";
        public const string Manage = "registration.manage";
        public const string Approve = "registration.approve";
    }

    public static class Participant
    {
        public const string View = "participant.view";
        public const string Approve = "participant.approve";
    }

    public static class Attendance
    {
        public const string View = "attendance.view";
        public const string Manage = "attendance.manage";
    }

    public static class Schedule
    {
        public const string View = "schedule.view";
        public const string Manage = "schedule.manage";
    }

    public static class Media
    {
        public const string View = "media.view";
        public const string Manage = "media.manage";
    }
}
