
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Abstractions.Services;
using EventFlow.Identity.Application.Behaviors;
using EventFlow.Identity.Application.Commands.RegisterUser;
using EventFlow.Identity.Infrastructure.Persistence;
using EventFlow.Identity.Infrastructure.Repositories;
using EventFlow.Identity.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityDatabase"));
            });
            builder.Services.AddMediatR(option =>
            {
                option.RegisterServicesFromAssemblies(typeof(RegisterUserCommand).Assembly);
            });
            builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);
            
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>),typeof(ValidationBehavior<,>));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
