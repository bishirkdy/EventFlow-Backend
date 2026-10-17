namespace EventFlow.Security.Authorization;

public static class PermissionConstants
{
    public static class Event
    {
        public const string Create = "event.create";
        public const string View = "event.view";
        public const string Update = "event.update";
        public const string TeamManage = "event.team.manage";
        public const string Preview = "event.preview";
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

    public static class Photo
    {
        public const string Upload = "photo.upload";
        public const string View = "photo.view";
        public const string Manage = "photo.manage";
    }
}
