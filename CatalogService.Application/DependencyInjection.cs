using CatalogService.Application.Common.AutoMapper;
using CatalogService.Application.Mediator.Behaviors;
using CatalogService.Application.RabbitMQ;
using EventBus.Abstraction.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SharedCollection.Events;
using System.Reflection;

namespace CatalogService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile(new ProductProfile());
            });

            services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddTransient<IIntegrationEventHandler<OrderConfirmedEvent>, OrderConfirmedHandler>();

            return services;
        }
    }
}
