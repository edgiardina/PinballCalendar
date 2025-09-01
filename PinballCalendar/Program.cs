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
    
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("WPPR API key is not configured. Please set the 'WPPRKey' in appsettings.json or environment variables.");
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