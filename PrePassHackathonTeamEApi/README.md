# PrePass Hackathon Team E API

A .NET Core Web API project developed for the PrePass Hackathon, featuring Family Members and Calendar Events management.

## Features

- Family Members Management
- Calendar Events Management
- In-memory Caching
- JWT Authentication
- Swagger/OpenAPI Documentation

## API Endpoints

### Family Members API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/FamilyMembers` | Get all family members (Mom, Dad, Lucy, Joe) |
| GET | `/api/FamilyMembers/{id}` | Get a specific family member |
| POST | `/api/FamilyMembers` | Create a new family member |
| PUT | `/api/FamilyMembers/{id}` | Update an existing family member |
| DELETE | `/api/FamilyMembers/{id}` | Delete a family member |

### Calendar Events API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/CalendarEvents` | Get all calendar events |
| POST | `/api/CalendarEvents` | Create a new calendar event |
| PUT | `/api/CalendarEvents/{id}` | Update an existing calendar event |
| DELETE | `/api/CalendarEvents/{id}` | Delete a calendar event |

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### Running the Application

1. Clone the repository
2. Navigate to the project directory:
   ```bash
   cd PrePassHackathonTeamEApi
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at:
- http://localhost:5000
- https://localhost:7019

## API Documentation

Swagger UI is available at `/swagger` when running the application. This provides interactive documentation for testing all endpoints.

### Sample Requests

#### Get All Family Members