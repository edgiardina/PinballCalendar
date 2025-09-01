# PinballCalendar API - .NET 9 Upgrade

This project has been successfully upgraded from .NET Framework 4.8 to .NET 9.

## Changes Made During Upgrade

### Project Structure
- ? Replaced old .NET Framework project file with modern SDK-style project file
- ? Migrated from `Web.config` to `appsettings.json` for configuration
- ? Replaced `Global.asax` with `Program.cs` for application startup
- ? Removed ASP.NET Web API Help Pages in favor of Swagger/OpenAPI
- ? Updated controller to use ASP.NET Core conventions
- ? Implemented Dependency Injection for services
- ? Uses System.Text.Json instead of Newtonsoft.Json for better performance

### Key Changes
1. **Configuration**: WPPR API key is now configured in `appsettings.json` instead of `Web.config`
2. **Dependency Injection**: 
   - `IPinballRankingApi` registered as scoped service with API key from configuration
   - `IGeocodingService` registered with HttpClient factory pattern
   - Configuration is injected into services automatically
3. **Error Handling**: Improved error handling with proper HTTP status codes
4. **API Documentation**: Swagger UI replaces the old Help Pages
5. **CORS**: Added CORS support for cross-origin requests
6. **JSON Serialization**: Uses System.Text.Json (built-in, faster than Newtonsoft.Json)

### Dependencies Updated
- `Ical.Net`: Updated to version 5.1.0 (breaking changes: `Event` ? `CalendarEvent`)
- `PinballApi`: Updated to version 3.1.15
- `Flurl.Http`: Updated to version 4.0.2
- Added `Swashbuckle.AspNetCore` for API documentation
- **Removed**: Newtonsoft.Json (using System.Text.Json instead)

### Dependency Injection Services
- **IGeocodingService**: Uses NominatimGeocodingService for address geocoding
- **IPinballRankingApi**: PinballRankingApi service with automatic API key injection
- **HttpClient**: Registered for HTTP operations

### Ical.Net 5.x Breaking Changes
- `Event` class renamed to `CalendarEvent`
- Duration calculations now use `CalDateTime.Value` property
- Import namespace `Ical.Net.CalendarComponents` for `CalendarEvent`

## Configuration

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "WPPRKey": "{ADD YOUR WPPRKEY HERE}"
}
```

**Important**: Replace `{ADD YOUR WPPRKEY HERE}` with your actual WPPR API key.

## Running the Application

### Prerequisites
- .NET 9 SDK installed
- WPPR API key

### Commands
```bash
# Restore packages
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

### Development
```bash
# Run in development mode with hot reload
dotnet watch run
```

## API Endpoints

### Get Calendar
```
GET /api/calendar/{address}/{distance}?showLeagues={true|false}
```

**Parameters:**
- `address`: The address to search around
- `distance`: Distance in miles
- `showLeagues`: (optional) Whether to include league events (default: true)

**Response:** Returns an iCalendar (.ics) file with pinball tournament events.

## API Documentation

When running in development mode, visit:
- Swagger UI: `https://localhost:5001/swagger` (or the port shown in console)
- OpenAPI spec: `https://localhost:5001/swagger/v1/swagger.json`

## Deployment

This .NET 9 application can be deployed to:
- Azure App Service
- AWS Elastic Beanstalk
- Docker containers
- IIS (with ASP.NET Core Module)
- Linux servers with reverse proxy (nginx/Apache)

## Environment Variables

For production deployment, you can set configuration via environment variables:
- `WPPRKey`: Your WPPR API key
- `ASPNETCORE_ENVIRONMENT`: Set to "Production" for production deployment

## Benefits of .NET 9 Upgrade

1. **Performance**: Significant performance improvements over .NET Framework
2. **Cross-platform**: Can run on Windows, Linux, and macOS
3. **Modern tooling**: Better development experience with modern .NET tooling
4. **Security**: Latest security updates and features
5. **Cloud-ready**: Better support for containerization and cloud deployment
6. **Long-term support**: Regular updates and long-term support from Microsoft
7. **Dependency Injection**: Built-in DI container for better testability and maintainability
8. **System.Text.Json**: Faster, more efficient JSON serialization than Newtonsoft.Json