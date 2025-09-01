using AWSEventDrivenDemo.Infrastructure.Messaging;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AWS SNS client and SnsEventPublisher
builder.Services.AddSingleton<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>();
builder.Services.AddSingleton<SnsEventPublisher>(sp =>
{
    var snsClient = sp.GetRequiredService<IAmazonSimpleNotificationService>();
    var logger = sp.GetRequiredService<ILogger<SnsEventPublisher>>();
    // Replace with your actual SNS Topic ARN or set via environment variable/appsettings
    var topicArn = builder.Configuration["SNS_TOPIC_ARN"] ?? "your-topic-arn";
    return new SnsEventPublisher(snsClient, topicArn, logger);
});

var app = builder.Build();

// Enable Swagger UI in all environments
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
