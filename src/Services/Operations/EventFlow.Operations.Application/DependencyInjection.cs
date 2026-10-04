using EventFlow.Operations.Application.Behaviors;
using EventFlow.Operations.Application.Features.Attendance;
using EventFlow.Operations.Application.Features.AttendanceStaff;
using EventFlow.Operations.Application.Features.Notifications;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Operations.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddOperationsApplication(this IServiceCollection services)
    {
        services.AddMediatR(
            options =>
            {
                options.RegisterServicesFromAssembly(
                    typeof(DependencyInjection).Assembly);

                options.AddOpenBehavior(
                    typeof(ValidationBehavior<,>));
            });

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);

        services.AddScoped<AttendanceService>();
        services.AddScoped<AttendanceStaffService>();
        services.AddScoped<NotificationService>();

        return services;
    }
}
