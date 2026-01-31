# Development Guide

## Project Architecture

### Backend (ASP.NET Core)

```
server/
├── Controllers/        # API endpoints
├── Services/          # Business logic
├── Models/            # Database entities
├── DTOs/              # Data transfer objects
├── Data/              # Database context
└── Program.cs         # Application entry point
```

### Frontend (React)

```
client/src/
├── components/        # Reusable UI components
├── pages/            # Route pages
├── services/         # API integration
├── utils/            # Helper functions
└── App.jsx           # Main application
```

## Adding New Features

### Backend: Adding a New Endpoint

1. **Create DTO** in `DTOs/`
2. **Add Service Method** in appropriate service
3. **Create Controller Action** in appropriate controller
4. **Test with Swagger**

Example:

```csharp
// Service
public async Task<MyDto> DoSomethingAsync(int id)
{
    // Implementation
}

// Controller
[HttpGet("{id}/something")]
public async Task<ActionResult<MyDto>> DoSomething(int id)
{
    var result = await _service.DoSomethingAsync(id);
    return Ok(result);
}
```

### Frontend: Adding a New Page

1. **Create page component** in `src/pages/`
2. **Add route** in `App.jsx`
3. **Create API service** if needed
4. **Add navigation link**

## Code Style

### Backend (.NET)

- Use PascalCase for classes, methods, properties
- Use camelCase for parameters and local variables
- Follow Microsoft C# conventions

### Frontend (JavaScript/React)

- Use camelCase for variables and functions
- Use PascalCase for component names
- Use Tailwind utility classes for styling

## Database Changes

### Creating Migrations

```bash
# Add new migration
dotnet ef migrations add AddNewFeature

# Update database
dotnet ef database update

# Rollback if needed
dotnet ef migrations remove
```

## Testing

### Backend Testing

Create tests in a separate test project:

```bash
dotnet new xunit -n ExpenseSplitter.Tests
```

### Frontend Testing

```bash
npm install --save-dev @testing-library/react @testing-library/jest-dom
npm test
```

## Performance Optimization

### Backend

- Use async/await for all I/O operations
- Implement caching for frequently accessed data
- Use database indexes appropriately
- Enable response compression

### Frontend

- Lazy load components with React.lazy()
- Memoize expensive calculations
- Optimize re-renders with useMemo and useCallback
- Implement virtual scrolling for large lists

## Security Best Practices

### Backend

- Always validate user input
- Use parameterized queries (EF Core does this)
- Implement proper authorization checks
- Log security-related events
- Rate limit API endpoints

### Frontend

- Never store sensitive data in localStorage
- Validate and sanitize user input
- Implement CSRF protection
- Use HTTPS in production

## Deployment

### Backend Deployment

**Using Azure App Service:**

```bash
# Publish
dotnet publish -c Release -o ./publish

# Deploy using Azure CLI
az webapp up --name your-app-name
```

### Frontend Deployment

**Using Vercel:**

```bash
# Install Vercel CLI
npm i -g vercel

# Deploy
vercel
```

## Environment Variables

### Backend (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "JwtSettings": {
    "Secret": "...",
    "Issuer": "...",
    "Audience": "...",
    "ExpiryInHours": 24
  }
}
```

### Frontend (.env)

```env
VITE_API_URL=http://localhost:5000/api
```

## Git Workflow

```bash
# Create feature branch
git checkout -b feature/new-feature

# Make changes and commit
git add .
git commit -m "Add new feature"

# Push to remote
git push origin feature/new-feature

# Create pull request
# Merge after review
```

## Common Tasks

### Reset Database

```bash
dotnet ef database drop
dotnet ef database update
```

### Clear Frontend Cache

```bash
rm -rf node_modules
rm package-lock.json
npm install
```

### Update Dependencies

**Backend:**

```bash
dotnet list package --outdated
dotnet add package PackageName --version x.x.x
```

**Frontend:**

```bash
npm outdated
npm update
```

## Debugging

### Backend

- Use Visual Studio debugger (F5)
- Set breakpoints in code
- Watch variables in debug window
- Check Output window for logs

### Frontend

- Use browser DevTools (F12)
- React DevTools extension
- Console.log for quick debugging
- Network tab for API calls

## Resources

- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)
- [React Best Practices](https://react.dev/learn/thinking-in-react)
- [Entity Framework Core Tips](https://docs.microsoft.com/en-us/ef/core/performance/)
