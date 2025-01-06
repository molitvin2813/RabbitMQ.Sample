﻿using EventBus.Abstraction.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Common.AutoMapper;
using OrderService.Application.Mediator.Behaviors;
using OrderService.Application.RabbitMQ;
using SharedCollection.Events;
using System.Reflection;

namespace OrderService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile(new BasketProfile());
                config.AddProfile(new OrderProfile());
            });

            services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddTransient<IIntegrationEventHandler<ProductUpdateEvent>, ProductUpdateHandler>();

            return services;
        }
    }
}
