using PinballApi;
using PinballApi.Interfaces;
using PinballCalendar.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpClient for services
builder.Services.AddHttpClient();

// Register Geocoding Service
builder.Services.AddHttpClient<IGeocodingService, NominatimGeocodingService>();

// Register PinballRankingApi
builder.Services.AddScoped<IPinballRankingApi>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var apiKey = configuration["WPPRKey"];
    
    // Debug logging to help troubleshoot UserSecrets
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("WPPRKey from configuration: {ApiKey}", string.IsNullOrEmpty(apiKey) ? "[NOT SET]" : "[SET]");
    
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("WPPR API key is not configured. Please set the 'WPPRKey' in appsettings.json or user secrets.");
    }
    
    return new PinballRankingApi(apiKey);
});

// Add CORS support if needed
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Log configuration sources for debugging (only in Development)
if (app.Environment.IsDevelopment())
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var configuration = app.Services.GetRequiredService<IConfiguration>();
    
    logger.LogInformation("Configuration sources:");
    if (configuration is IConfigurationRoot configRoot)
    {
        foreach (var provider in configRoot.Providers)
        {
            logger.LogInformation("- {ProviderType}", provider.GetType().Name);
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseRouting();
app.UseCors();

app.MapControllers();

app.Run();