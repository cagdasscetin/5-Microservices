using MassTransit;
using Micro.Application.Interfaces.Repositories;
using Micro.Persistence.Context;
using Micro.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Quartz;
using OutboxWorker.Jobs;
using Polly;
using Polly.Registry;

var circuitBreakerPolicy = Policy
    .Handle<Exception>()
    .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10),
        onBreak: (exception, breakDelay) =>
        {
            Console.WriteLine($"Circuit broken for {breakDelay.TotalSeconds} seconds due to {exception.Message}");
        },
        onReset: () => {
            Console.WriteLine("Circuit reset.");
        },
        onHalfOpen: () => {
            Console.WriteLine("Circuit is half-open. Next call is a trial.");
        }
    );

var retryPolicy = Policy
    .Handle<Exception>()
    .RetryAsync(3, (exception, retryCount) =>
    {
        Console.WriteLine($"Retrying due to: {exception.Message}. Attempt: {retryCount}");
    });

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IPolicyRegistry<string> registry = services.AddPolicyRegistry();
        registry.Add("CircuitBreakerPolicy", circuitBreakerPolicy);
        registry.Add("RetryPolicy", retryPolicy);
        
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));
        
        services.AddMassTransit(configurator =>
        {
            configurator.UsingRabbitMq((context, _configure) =>
            {
                var host = hostContext.Configuration.GetSection("RabbitMQ:Host").Value;
                _configure.Host(host, "/", h =>
                {
                    h.Username(hostContext.Configuration.GetSection("RabbitMQ:Username").Value);
                    h.Password(hostContext.Configuration.GetSection("RabbitMQ:Password").Value);
                });
            });
        });
        
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey = new JobKey("MyRecurringJob");

            q.AddJob<OutboxJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => 
                opts.ForJob(jobKey)
                    .WithIdentity("MyRecurringJob-trigger")
                    .WithSimpleSchedule(x => 
                        x.WithIntervalInSeconds(10)
                            .RepeatForever()));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        services.AddHealthChecks();
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.Configure((context, app) =>
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            
            // Map health checks to the desired endpoint
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health"); // Health check endpoint
            });
        });
    })
    .Build();

await host.RunAsync();