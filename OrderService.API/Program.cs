using EventBus.RabbitMQ;
using EventBus.RabbitMQ.Settings;
using OrderService.API.Jobs;
using OrderService.Application;
using OrderService.PostgreSQL;
using Quartz;

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
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever())
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

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}