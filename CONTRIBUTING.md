# Contributing to Expense Splitter

Thank you for considering contributing to Expense Splitter! This document provides guidelines for contributing.

## How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported
2. Use the issue template (if available)
3. Include:
   - Clear description
   - Steps to reproduce
   - Expected vs actual behavior
   - Screenshots if applicable
   - Environment details (OS, browser, .NET version)

### Suggesting Features

1. Check if feature has been suggested
2. Describe the feature clearly
3. Explain use case and benefits
4. Provide examples if possible

### Pull Requests

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Make your changes**
   - Follow code style guidelines
   - Add tests if applicable
   - Update documentation
4. **Commit your changes**
   ```bash
   git commit -m "Add: description of your changes"
   ```
5. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```
6. **Create Pull Request**
   - Describe changes clearly
   - Reference related issues
   - Ensure all tests pass

## Code Style Guidelines

### Backend (.NET/C#)

- Follow Microsoft C# conventions
- Use PascalCase for public members
- Use camelCase for private fields (with underscore prefix)
- Add XML documentation for public APIs
- Use async/await for I/O operations

Example:

```csharp
/// <summary>
/// Creates a new expense for the specified group
/// </summary>
/// <param name="dto">Expense creation data</param>
/// <returns>Created expense details</returns>
public async Task<ExpenseDto> CreateExpenseAsync(CreateExpenseDto dto)
{
    // Implementation
}
```

### Frontend (React/JavaScript)

- Use functional components
- Use hooks for state management
- Name components in PascalCase
- Use camelCase for functions and variables
- Add PropTypes or TypeScript types

Example:

```javascript
function ExpenseCard({ expense, onDelete }) {
  const handleDelete = async () => {
    // Implementation
  };

  return <div className="card">{/* Component content */}</div>;
}
```

## Git Commit Messages

Use conventional commit format:

- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `style:` Code style changes (formatting)
- `refactor:` Code refactoring
- `test:` Adding tests
- `chore:` Maintenance tasks

Examples:

```
feat: Add expense filtering by date
fix: Resolve balance calculation error
docs: Update API documentation
refactor: Simplify settlement algorithm
```

## Testing

### Backend Tests

```bash
cd server
dotnet test
```

### Frontend Tests

```bash
cd client
npm test
```

## Code Review Process

1. At least one approval required
2. All CI checks must pass
3. Code must follow style guidelines
4. Tests must be included for new features
5. Documentation must be updated

## Development Workflow

1. Pick an issue or create one
2. Assign yourself to the issue
3. Create feature branch
4. Develop and test locally
5. Push changes
6. Create pull request
7. Address review comments
8. Merge after approval

## Questions?

- Open an issue for questions
- Check existing documentation
- Review closed issues for similar problems

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

Thank you for contributing! 🎉
