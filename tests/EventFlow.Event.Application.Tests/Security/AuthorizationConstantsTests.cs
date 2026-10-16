using EventFlow.Security.Authorization;

namespace EventFlow.Event.Application.Tests.Security;

public sealed class AuthorizationConstantsTests
{
    private static readonly string[] AllPermissions =
    [
        PermissionConstants.Event.Create,
        PermissionConstants.Event.View,
        PermissionConstants.Event.Update,
        PermissionConstants.Event.TeamManage,
        PermissionConstants.Participant.View,
        PermissionConstants.Participant.Approve,
        PermissionConstants.Attendance.View,
        PermissionConstants.Attendance.Manage,
        PermissionConstants.Photo.Upload,
        PermissionConstants.Photo.View,
        PermissionConstants.Photo.Manage,
    ];

    private static readonly string[] AllRoles =
    [
        RoleConstants.Organizer,
        RoleConstants.EventOwner,
        RoleConstants.EventAdministrator,
        RoleConstants.Staff,
        RoleConstants.AttendanceStaff,
        RoleConstants.Photographer,
        RoleConstants.Participant,
    ];

    public static TheoryData<string> Permissions
    {
        get
        {
            var data = new TheoryData<string>();
            foreach (var permission in AllPermissions)
            {
                data.Add(permission);
            }
            return data;
        }
    }

    public static TheoryData<string> Roles
    {
        get
        {
            var data = new TheoryData<string>();
            foreach (var role in AllRoles)
            {
                data.Add(role);
            }
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(Permissions))]
    public void Permission_names_are_not_blank(string permission)
    {
        Assert.False(string.IsNullOrWhiteSpace(permission));
    }

    [Fact]
    public void Permission_names_are_unique()
    {
        Assert.Equal(AllPermissions.Length, AllPermissions.Distinct().Count());
    }

    [Theory]
    [MemberData(nameof(Roles))]
    public void Role_names_are_not_blank(string role)
    {
        Assert.False(string.IsNullOrWhiteSpace(role));
    }

    [Fact]
    public void Role_names_are_unique()
    {
        Assert.Equal(AllRoles.Length, AllRoles.Distinct().Count());
    }

    [Fact]
    public void Event_owner_role_matches_the_seeded_name()
    {
        // The database seeds the owner role as "Owner"; the constant must match.
        Assert.Equal("Owner", RoleConstants.EventOwner);
    }
}
