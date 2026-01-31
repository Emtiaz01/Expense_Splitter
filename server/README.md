# Backend Setup Instructions

## Quick Start

```bash
cd server
dotnet restore
dotnet ef database update
dotnet run
```

The API will start at `http://localhost:5000`

Swagger UI: `http://localhost:5000/swagger`

## Project Structure

```
server/
├── Controllers/        # API endpoints
│   ├── AuthController.cs
│   ├── GroupsController.cs
│   ├── ExpensesController.cs
│   └── BalanceController.cs
├── Services/          # Business logic
│   ├── JwtService.cs
│   ├── AuthService.cs
│   ├── GroupService.cs
│   ├── ExpenseService.cs
│   └── BalanceService.cs
├── Models/            # Database entities
│   ├── User.cs
│   ├── Group.cs
│   ├── GroupMember.cs
│   ├── Expense.cs
│   ├── ExpenseSplit.cs
│   └── Settlement.cs
├── DTOs/              # Data transfer objects
│   ├── AuthDtos.cs
│   ├── GroupDtos.cs
│   ├── ExpenseDtos.cs
│   └── BalanceDtos.cs
├── Data/              # Database context
│   └── ApplicationDbContext.cs
├── Program.cs         # Application configuration
└── appsettings.json   # Configuration
```

## Configuration

### Database Connection

Edit `appsettings.json`:

**SQL Server:**

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ExpenseSplitter;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**PostgreSQL:**

```json
"ConnectionStrings": {
  "PostgreSQL": "Host=localhost;Database=ExpenseSplitter;Username=postgres;Password=your_password"
}
```

### JWT Settings

```json
"JwtSettings": {
  "Secret": "YourSuperSecretKeyHere!",
  "Issuer": "ExpenseSplitterAPI",
  "Audience": "ExpenseSplitterClient",
  "ExpiryInHours": 24
}
```

## Database Migrations

### Create Migration

```bash
dotnet ef migrations add MigrationName
```

### Update Database

```bash
dotnet ef database update
```

### Rollback Migration

```bash
dotnet ef database update PreviousMigrationName
```

### Remove Last Migration

```bash
dotnet ef migrations remove
```

## NuGet Packages

Core packages used:

- **Microsoft.EntityFrameworkCore.SqlServer** - SQL Server support
- **Npgsql.EntityFrameworkCore.PostgreSQL** - PostgreSQL support
- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT auth
- **BCrypt.Net-Next** - Password hashing
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI

## API Endpoints

See `docs/API.md` for complete API documentation.

Key endpoints:

- `POST /api/auth/register` - Register user
- `POST /api/auth/login` - Login
- `GET /api/groups` - Get user's groups
- `POST /api/expenses` - Create expense
- `GET /api/groups/{id}/balances` - Get balances

## Testing with Swagger

1. Run the application
2. Navigate to `http://localhost:5000/swagger`
3. Click "Authorize" button
4. Enter: `Bearer your_jwt_token`
5. Test endpoints

## Authentication Flow

1. User registers or logs in
2. Server returns JWT token
3. Client includes token in Authorization header
4. Server validates token on each request
5. Token contains userId, used to identify user

## Security Features

- Password hashing with BCrypt
- JWT-based authentication
- Role-based authorization (Admin/Member)
- Input validation on all DTOs
- SQL injection protection (EF Core)
- CORS configuration

## Development Tips

### Hot Reload

```bash
dotnet watch run
```

### Check for Updates

```bash
dotnet list package --outdated
```

### Clean and Rebuild

```bash
dotnet clean
dotnet build
```

## Deployment

### Publish for Production

```bash
dotnet publish -c Release -o ./publish
```

### Environment Variables

Set these in production:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection`
- `JwtSettings__Secret`

## Common Issues

**Issue: Database connection failed**

- Verify SQL Server is running
- Check connection string
- Test connection in SSMS

**Issue: Port 5000 already in use**

- Change port in `Properties/launchSettings.json`
- Or kill process: `netstat -ano | findstr :5000`

**Issue: Migration failed**

- Drop database and recreate
- Check model relationships
- Verify foreign keys

## Logging

Logs are configured in `appsettings.json`:

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```

## Performance

- Use async/await for all I/O operations
- Enable response compression
- Implement caching where appropriate
- Add database indexes
- Use pagination for large datasets

## Best Practices

1. Always use DTOs for API contracts
2. Keep controllers thin, business logic in services
3. Use dependency injection
4. Write unit tests for services
5. Document API with XML comments
