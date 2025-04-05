# 🚌 Public Transport Planner API

A C# ASP.NET Core Web API for planning public transport routes. Calculate transit times between multiple addresses and get address suggestions using Google Maps APIs.

## ✨ Features

- 🔍 Address autocomplete with Google Places API
- 🕖 Public transport time calculation between multiple points
- 📊 RESTful API design with proper error handling
- 📝 Comprehensive Swagger documentation
- 🔄 Hot-reload for efficient development

## 🛠️ Tech Stack

- [ASP.NET Core 9](https://dotnet.microsoft.com/en-us/apps/aspnet) - Web API Framework
- [C# 12](https://dotnet.microsoft.com/en-us/languages/csharp) - Programming Language
- [Swagger/OpenAPI](https://swagger.io/) - API Documentation
- [Google Maps Platform](https://developers.google.com/maps) - Places API & Distance Matrix API

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- Google Maps API key

### Installation

1. Clone the repository

```bash
git clone <your-repository-url>
cd PublicTransportPlannerApi
```

2. Configure your Google Maps API key

Create an `appsettings.Development.json` file for development settings (this file is excluded from git):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "GoogleMaps": {
    "ApiKey": "<your-maps-api-key>"
  }
}
```

You can also update the existing `appsettings.json` file, but be cautious not to commit your API key to version control:

3. Build and run the application

```bash
dotnet build
dotnet run
```

For development with hot-reload:

```bash
dotnet watch run
```

4. Access the API documentation

Open your browser and navigate to:
```
http://localhost:5279/
```

## 📡 API Endpoints

### Transit Calculations

```
POST /api/transit/calculate
```

Calculate transit times between multiple addresses.

### Places Autocomplete

```
GET /api/places/autocomplete
```

Get address suggestions based on user input.

## 📝 Code Documentation

This project follows C# best practices for code documentation. All public APIs include XML documentation that is automatically integrated with Swagger UI. The code comments and documentation for this project were enhanced using AI assistance.

## 📦 Project Structure

- **Controllers/** - API endpoint definitions
  - `PlacesController.cs` - Handles address autocomplete requests
  - `TransitController.cs` - Handles transit time calculations
- **Models/** - Data models and DTOs
- **Services/** - Business logic and external API integration
  - `GoogleMapsService.cs` - Integration with Google Maps APIs
  - `TransitService.cs` - Transit calculation logic

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.
