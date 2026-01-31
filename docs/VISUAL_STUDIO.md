## Visual Studio Setup

### Opening the Project

1. Navigate to the `server` folder
2. Double-click `ExpenseSplitter.csproj`
3. Or open folder in Visual Studio 2022

### First-Time Setup

1. **Restore NuGet Packages**
   - Right-click solution → Restore NuGet Packages
   - Or: Tools → NuGet Package Manager → Restore

2. **Configure Database**
   - Update connection string in `appsettings.json`
   - Open Package Manager Console:
     - Tools → NuGet Package Manager → Package Manager Console
   - Run:
     ```powershell
     Add-Migration InitialCreate
     Update-Database
     ```

3. **Build Solution**
   - Press `Ctrl+Shift+B`
   - Or: Build → Build Solution

4. **Run Application**
   - Press `F5` (with debugging)
   - Or `Ctrl+F5` (without debugging)

### Debugging

- Set breakpoints by clicking left margin
- Press `F5` to start debugging
- Use Debug → Windows → Locals to inspect variables
- Check Output window for logs

### Useful Shortcuts

- `F5` - Start debugging
- `Ctrl+F5` - Start without debugging
- `Shift+F5` - Stop debugging
- `F9` - Toggle breakpoint
- `F10` - Step over
- `F11` - Step into
- `Ctrl+K, Ctrl+D` - Format document
- `Ctrl+.` - Quick actions and refactorings

### Package Manager Console Commands

```powershell
# Entity Framework
Add-Migration MigrationName
Update-Database
Remove-Migration
Drop-Database

# NuGet
Install-Package PackageName
Update-Package PackageName
Uninstall-Package PackageName
```

### Solution Explorer Tips

- Right-click project → Manage NuGet Packages
- Right-click folder → Add → New Item
- Press `Ctrl+;` for solution-wide search

### Testing with Swagger

After running (F5):

1. Browser opens automatically
2. Navigate to `/swagger`
3. Test all endpoints

### Common Issues

**Issue: NuGet restore failed**

- Tools → Options → NuGet Package Manager → Clear All NuGet Cache(s)
- Close and reopen Visual Studio

**Issue: Can't debug**

- Check that `ASPNETCORE_ENVIRONMENT` is set to `Development`
- Verify `launchSettings.json` configuration

**Issue: Database errors**

- Check connection string
- Verify SQL Server is running
- Try `Drop-Database` then `Update-Database`

### Recommended Extensions

- **C# Dev Kit** - Enhanced C# experience
- **ReSharper** - Code analysis (paid)
- **EF Core Power Tools** - Visual EF Core tools
- **SQLite/SQL Server Compact Toolbox** - Database management

### Configuration Files

**launchSettings.json** - Located in `Properties/` folder

```json
{
  "profiles": {
    "ExpenseSplitter": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### Multi-Project Solution

To run both frontend and backend from Visual Studio:

1. Right-click solution → Properties
2. Common Properties → Startup Project
3. Select "Multiple startup projects"
4. Set both projects to "Start"

### Performance Profiling

- Debug → Performance Profiler
- Select "CPU Usage" or "Memory Usage"
- Start profiling
- Analyze results

### Git Integration

Visual Studio has built-in Git support:

- View → Git Changes
- Commit, push, pull from GUI
- View history and branches

### Tips for Productivity

1. Use **Live Share** for collaborative coding
2. Enable **Hot Reload** for instant updates
3. Use **IntelliSense** (Ctrl+Space) extensively
4. Organize imports: Ctrl+R, Ctrl+G
5. Use **Solution Filters** for large solutions
