

using EventFlow.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Persistence
{
    public static class IdentitySeedData
    {
        private static readonly DateTime SeedDate = new DateTime(2026, 9, 7, 0, 0, 0, DateTimeKind.Utc);
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Fixed IDs make seed data stable across migrations.
            var organizerRoleId = Guid.Parse("10000000-0000-0000-0000-000000000001");
            var eventAdminRoleId = Guid.Parse("10000000-0000-0000-0000-000000000002");
            var staffRoleId = Guid.Parse("10000000-0000-0000-0000-000000000003");
            var participantRoleId = Guid.Parse("10000000-0000-0000-0000-000000000004");

            var eventCreateId = Guid.Parse("20000000-0000-0000-0000-000000000001");
            var eventViewId = Guid.Parse("20000000-0000-0000-0000-000000000002");
            var eventUpdateId = Guid.Parse("20000000-0000-0000-0000-000000000003");

            var participantViewId = Guid.Parse("20000000-0000-0000-0000-000000000004");
            var participantApproveId = Guid.Parse("20000000-0000-0000-0000-000000000005");

            var attendanceViewId = Guid.Parse("20000000-0000-0000-0000-000000000006");
            var attendanceManageId = Guid.Parse("20000000-0000-0000-0000-000000000007");

            modelBuilder.Entity<Role>().HasData(
           new
           {
               Id = organizerRoleId,
               Name = "Organizer",
               Description = "Event organizer",
               CreatedAt = SeedDate
           },
           new
           {
               Id = eventAdminRoleId,
               Name = "EventAdmin",
               Description = "Event administrator",
               CreatedAt = SeedDate
           },
           new
           {
               Id = staffRoleId,
               Name = "Staff",
               Description = "Event staff",
               CreatedAt = SeedDate
           },
           new
           {
               Id = participantRoleId,
               Name = "Participant",
               Description = "Event participant",
               CreatedAt = SeedDate
           }
       );

            modelBuilder.Entity<Permission>().HasData(
                new
                {
                    Id = eventCreateId,
                    Name = "event.create",
                    Description = "Create event",
                    CreatedAt = SeedDate
                },
                new
                {
                    Id = eventViewId,
                    Name = "event.view",
                    Description = "View event",
                    CreatedAt = SeedDate
                },
                new
                {
                    Id = eventUpdateId,
                    Name = "event.update",
                    Description = "Update event",
                    CreatedAt = SeedDate
                },
                new
                {
                    Id = participantViewId,
                    Name = "participant.view",
                    Description = "View participants",
                    CreatedAt = SeedDate
                },
                new
                {
                    Id = participantApproveId,
                    Name = "participant.approve",
                    Description = "Approve participants",
                    CreatedAt = SeedDate
                },
                new
                {
                    Id = attendanceViewId,
                    Name = "attendance.view",
                    Description = "View attendance",
                    CreatedAt = SeedDate
                },
                new
                {
                    Id = attendanceManageId,
                    Name = "attendance.manage",
                    Description = "Manage attendance",
                    CreatedAt = SeedDate
                }
            );

            // Organizer permissions
            modelBuilder.Entity<RolePermission>().HasData(
                new
                {
                    RoleId = organizerRoleId,
                    PermissionId = eventCreateId
                },
                new
                {
                    RoleId = organizerRoleId,
                    PermissionId = eventViewId
                },
                new
                {
                    RoleId = organizerRoleId,
                    PermissionId = eventUpdateId
                },
                new
                {
                    RoleId = organizerRoleId,
                    PermissionId = participantViewId
                },
                new
                {
                    RoleId = organizerRoleId,
                    PermissionId = participantApproveId
                },
                new
                {
                    RoleId = organizerRoleId,
                    PermissionId = attendanceViewId
                },
                new
                {
                    RoleId = organizerRoleId,
                    PermissionId = attendanceManageId
                },

                // EventAdmin permissions
                new
                {
                    RoleId = eventAdminRoleId,
                    PermissionId = eventViewId
                },
                new
                {
                    RoleId = eventAdminRoleId,
                    PermissionId = eventUpdateId
                },
                new
                {
                    RoleId = eventAdminRoleId,
                    PermissionId = participantViewId
                },
                new
                {
                    RoleId = eventAdminRoleId,
                    PermissionId = participantApproveId
                },
                new
                {
                    RoleId = eventAdminRoleId,
                    PermissionId = attendanceViewId
                },
                new
                {
                    RoleId = eventAdminRoleId,
                    PermissionId = attendanceManageId
                },

                // Staff permissions
                new
                {
                    RoleId = staffRoleId,
                    PermissionId = participantViewId
                },
                new
                {
                    RoleId = staffRoleId,
                    PermissionId = attendanceViewId
                },
                new
                {
                    RoleId = staffRoleId,
                    PermissionId = attendanceManageId
                },

                // Participant permissions
                new
                {
                    RoleId = participantRoleId,
                    PermissionId = eventViewId
                }
            );
        }
    }

}
    
