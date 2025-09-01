# PinballCalendar API - .NET 9 

## Running the Application

### Prerequisites
- .NET 9 SDK installed
- WPPR API key

### Setup API Key
1. **Using UserSecrets (Recommended)**:
   ```bash
   dotnet user-secrets set "WPPRKey" "your-actual-api-key-here"
   ```

2. **Using Environment Variable**:
   ```bash
   set WPPRKey=your-actual-api-key-here  # Windows
   export WPPRKey=your-actual-api-key-here  # Linux/Mac
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

## Environment Variables

For production deployment, you can set configuration via environment variables:
- `WPPRKey`: Your WPPR API key
- `ASPNETCORE_ENVIRONMENT`: Set to "Production" for production deployment
