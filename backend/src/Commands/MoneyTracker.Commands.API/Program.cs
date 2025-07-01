using System.Diagnostics.CodeAnalysis;
using Amazon.S3;
using Amazon.SQS;
using Microsoft.OpenApi.Models;
using MoneyTracker.Authentication;
using MoneyTracker.Authentication.Authentication;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.AWS;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.PlatformService;

[ExcludeFromCodeCoverage]
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(config =>
        {
            config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            config.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            config.MapType<DateOnly>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "date"
            });
        });

        var databaseConnectionString = GetCliArgumentValue<string>(args, "--database") ?? builder.Configuration["Database:Paelagus_RO"]!;
        if (databaseConnectionString == null || databaseConnectionString == "")
        {
            throw new Exception("Database connection string must be set under --database");
        }
        if (DoesCliArgumentExist(args, "--reset-database"))
        {
            Migration.CheckMigration(databaseConnectionString, new MigrationOption(true, true));
        }

        Startup.Start(builder);

        if (!DoesCliArgumentExist(args, "--use-messaging"))
        {
            PlatformServiceStartup.StartEmptyClient(builder);
        }
        else
        {
            var rabbitConnectionString = GetCliArgumentValue<string>(args, "--rabbit") ?? builder.Configuration["Messaging:Lepus"]!;
            if (rabbitConnectionString != null && rabbitConnectionString != "")
            {
                PlatformServiceStartup.StartClient(builder, rabbitConnectionString);
            }
            else
            {
                throw new Exception("Messaging connection string must be set under --rabbit");
            }
        }

        DoAmazonStuff(builder, args);

        builder.Services
            .AddHttpContextAccessor()
            .AddSingleton<IDatabase>(_ => new PostgresDatabase(databaseConnectionString))
            .AddSingleton<IAuthenticationService, AuthenticationService>()
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IFrequencyCalculation, FrequencyCalculation>()
            .AddSingleton<IMonthDayCalculator, MonthDayCalculator>();

        builder.Services
            .AddSingleton<IAccountService, AccountService>()
            .AddSingleton<IAccountCommandRepository, AccountCommandRepository>();

        builder.Services
            .AddSingleton<IBillService, BillService>()
            .AddSingleton<IBillCommandRepository, BillCommandRepository>();

        builder.Services
            .AddSingleton<IBudgetService, BudgetService>()
            .AddSingleton<IBudgetCommandRepository, BudgetCommandRepository>();

        builder.Services
            .AddSingleton<ICategoryService, CategoryService>()
            .AddSingleton<ICategoryCommandRepository, CategoryCommandRepository>();

        builder.Services
            .AddSingleton<IReceiptCommandRepository, ReceiptCommandRepository>()
            .AddSingleton<IRegisterService, RegisterService>()
            .AddSingleton<IRegisterCommandRepository, RegisterCommandRepository>();

        builder.Services
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<IUserCommandRepository, UserCommandRepository>();

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

    private static T? GetCliArgumentValue<T>(string[] args, string key) where T : class
    {
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (args[index] == key)
            {
                return (T)Convert.ChangeType(args[index + 1], typeof(T));
            }
        }

        return null;
    }

    private static bool DoesCliArgumentExist(string[] args, string key)
    {
        return args.Any(x => x == key);
    }

    private static void DoAmazonStuff(WebApplicationBuilder builder, string[] args)
    {
        builder.Services
            .AddDefaultAWSOptions(builder.Configuration.GetAWSOptions())
            .AddAWSService<IAmazonS3>()
            .AddAWSService<IAmazonSQS>();

        var preprocessBucketName = GetCliArgumentValue<string>(args, "--aws-preprocess-bucket") ?? builder.Configuration["AWS:PreprocessBucket"]!;
        builder.Services
            .AddSingleton<IFileUploadRepository>(provider =>
                new S3Repository(provider.GetRequiredService<IAmazonS3>(), preprocessBucketName));

        // Polling
        var postprocessBucketName = GetCliArgumentValue<string>(args, "--aws-postprocess-bucket") ?? builder.Configuration["AWS:PostprocessBucket"]!;
        var sqsUrl = GetCliArgumentValue<string>(args, "--aws-sqs-url") ?? builder.Configuration["AWS:SQSUrl"]!;
        builder.Services
            .AddSingleton<IPollingController, PollingController>()
            .AddSingleton<IMessageQueueService>(provider => new MessageQueueService(
                provider.GetRequiredService<IMessageQueueRepository>(),
                provider.GetRequiredService<IReceiptCommandRepository>(),
                new S3Repository(provider.GetRequiredService<IAmazonS3>(), postprocessBucketName),
                provider.GetRequiredService<IPollingController>()))
            .AddSingleton<IMessageQueueRepository>(provider => new SQSRepository(provider.GetRequiredService<IAmazonSQS>(), sqsUrl, 5));

        builder.Services.AddHostedService<MessagePollingWorker>();
    }
}
