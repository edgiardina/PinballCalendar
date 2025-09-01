# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["PinballCalendar.csproj", "."]
RUN dotnet restore "./PinballCalendar.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "PinballCalendar.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PinballCalendar.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PinballCalendar.dll"]