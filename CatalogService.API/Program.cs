using CatalogService.API.Jobs;
using CatalogService.Application;
using CatalogService.Application.RabbitMQ;
using CatalogService.PostgreSQL;
using EventBus.Abstraction.Interfaces;
using EventBus.RabbitMQ;
using EventBus.RabbitMQ.Settings;
using Quartz;
using SharedCollection.Events;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddApplication();
        builder.Services.AddEventBus();
        builder.Services.AddPostgresDB(builder.Configuration);

        builder.Services.AddEventBus();

        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection(nameof(RabbitMQSettings)));

        builder.Services.AddQuartz(config =>
        {
            config.AddJob<PublishOutboxJob>(opts =>
            {
                opts.WithIdentity(PublishOutboxJob.JobKey);

            });

            config.AddTrigger(opts => opts
                .ForJob(PublishOutboxJob.JobKey)
                .WithIdentity(PublishOutboxJob.TriggerKey)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(20).RepeatForever())
            );

            config.AddJob<ProcessOutboxJob>(opts =>
            {
                opts.WithIdentity(ProcessOutboxJob.JobKey);

            });

            config.AddTrigger(opts => opts
                .ForJob(ProcessOutboxJob.JobKey)
                .WithIdentity(ProcessOutboxJob.TriggerKey)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever())
            );
        });

        builder.Services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        var gg = app.Services.GetService<IEventBus>();

        gg.Subscribe<OrderConfirmedEvent, OrderConfirmedHandler>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}