# 🚀 Setup Instructions - Expense Splitter

## Prerequisites

Before you begin, ensure you have the following installed:

### Required Software

- **Node.js** (v18 or higher) - [Download](https://nodejs.org/)
- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** (Community Edition or higher) - [Download](https://visualstudio.microsoft.com/)
- **SQL Server** (or PostgreSQL) - [Download SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Optional Tools

- **Postman** or **Thunder Client** (for API testing)
- **Git** for version control

---

## 📁 Project Structure

```
expense-splitter/
├── client/              # React frontend (Vite)
├── server/              # ASP.NET Core backend
├── database/            # Database schema
├── docs/                # Documentation
└── README.md
```

---

## ⚙️ Backend Setup (ASP.NET Core)

### Step 1: Database Configuration

#### Option A: Using SQL Server (Recommended for Windows)

1. **Install SQL Server**
   - Download and install SQL Server Express
   - Install SQL Server Management Studio (SSMS) for easier management

2. **Update Connection String**

   Open `server/appsettings.json` and update:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=ExpenseSplitter;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

   For SQL Server with username/password:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=ExpenseSplitter;User Id=your_username;Password=your_password;TrustServerCertificate=True;"
   }
   ```

#### Option B: Using PostgreSQL

1. **Install PostgreSQL**
   - Download and install PostgreSQL

2. **Update Program.cs**

   In `server/Program.cs`, comment out SQL Server and uncomment PostgreSQL:

   ```csharp
   // Comment this:
   // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

   // Uncomment this:
   options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
   ```

3. **Update Connection String**

   In `server/appsettings.json`:

   ```json
   "ConnectionStrings": {
     "PostgreSQL": "Host=localhost;Database=ExpenseSplitter;Username=postgres;Password=your_password"
   }
   ```

### Step 2: Update JWT Secret

In `server/appsettings.json`, change the JWT secret to something unique:

```json
"JwtSettings": {
  "Secret": "YourUniqueSecretKeyThatIsAtLeast32CharactersLongForProduction!",
  "Issuer": "ExpenseSplitterAPI",
  "Audience": "ExpenseSplitterClient",
  "ExpiryInHours": 24
}
```

⚠️ **Important**: Never commit your production secrets to Git!

### Step 3: Open in Visual Studio

1. Navigate to the `server` folder
2. Open `ExpenseSplitter.csproj` or the folder in Visual Studio 2022
3. Visual Studio will automatically restore NuGet packages

### Step 4: Create Database

#### Method 1: Using EF Core Migrations (Recommended)

Open Package Manager Console in Visual Studio (Tools → NuGet Package Manager → Package Manager Console):

```powershell
# Create initial migration
Add-Migration InitialCreate

# Apply migration to database
Update-Database
```

#### Method 2: Using .NET CLI

Open terminal in the `server` folder:

```bash
# Install EF Core tools globally (one-time)
dotnet tool install --global dotnet-ef

# Create migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

#### Method 3: Using SQL Script

Run the SQL script from `database/schema.sql` in SSMS or your database client.

### Step 5: Run the Backend

#### In Visual Studio:

- Press **F5** or click the **Run** button
- The API will start on `https://localhost:5001` or `http://localhost:5000`

#### Using .NET CLI:

```bash
cd server
dotnet run
```

### Step 6: Test the API

1. Open browser and navigate to: `http://localhost:5000/swagger`
2. You should see the Swagger UI with all API endpoints
3. Try the health check endpoint

---

## 🎨 Frontend Setup (React + Vite)

### Step 1: Install Dependencies

Open a terminal in the `client` folder:

```bash
cd client
npm install
```

This will install all required packages:

- React & React DOM
- React Router
- Axios
- Chart.js
- jsPDF
- Tailwind CSS
- And more...

### Step 2: Configure API URL (Optional)

If your backend runs on a different port, create `.env` file in the `client` folder:

```env
VITE_API_URL=http://localhost:5000/api
```

By default, it uses the proxy configured in `vite.config.js`.

### Step 3: Run the Frontend

```bash
npm run dev
```

The app will start on `http://localhost:3000` (or `http://localhost:5173` depending on Vite version).

### Step 4: Build for Production (Optional)

```bash
npm run build
```

Production files will be in the `client/dist` folder.

---

## 🔄 Running Both Frontend and Backend

### Option 1: Two Terminals

**Terminal 1 (Backend):**

```bash
cd server
dotnet run
```

**Terminal 2 (Frontend):**

```bash
cd client
npm run dev
```

### Option 2: Visual Studio + Terminal

1. Run backend from Visual Studio (F5)
2. Open terminal for frontend:
   ```bash
   cd client
   npm run dev
   ```

---

## 🧪 Testing the Application

### 1. Register a User

1. Open `http://localhost:3000` in your browser
2. Click "Register here"
3. Fill in:
   - Name: Your Name
   - Email: your@email.com
   - Password: password123
4. Click "Register"

### 2. Create a Group

1. After login, click "+ Create Group"
2. Enter:
   - Group Name: "Trip to Goa"
   - Description: "Beach vacation"
3. Click "Create Group"

### 3. Add Expenses

1. Click on your group
2. Click "+ Add Expense"
3. Fill in expense details
4. Choose split type
5. Select participants
6. Click "Add Expense"

### 4. View Balances

1. Go to "Balances" tab
2. See who owes whom
3. View the balance chart

### 5. View Settlements

1. Click "View Settlements"
2. See optimized payment suggestions
3. Mark payments as paid
4. Export PDF report

---

## 🐛 Troubleshooting

### Backend Issues

**Issue: Database connection failed**

- Check if SQL Server is running
- Verify connection string in `appsettings.json`
- Try `dotnet ef database update` again

**Issue: Port already in use**

- Change port in `Properties/launchSettings.json`
- Or kill the process using the port

**Issue: NuGet packages not restoring**

```bash
dotnet restore
```

### Frontend Issues

**Issue: npm install fails**

- Delete `node_modules` and `package-lock.json`
- Run `npm install` again
- Try `npm install --legacy-peer-deps`

**Issue: Can't connect to backend**

- Check if backend is running
- Verify `VITE_API_URL` in `.env`
- Check CORS settings in `Program.cs`

**Issue: Tailwind styles not working**

- Delete `.next` or `dist` folder
- Run `npm run dev` again

---

## 📦 Deployment

### Backend (Azure / Railway / Render)

1. **Publish the application:**

   ```bash
   dotnet publish -c Release
   ```

2. **Update connection string** in production `appsettings.json`

3. **Set environment variables:**
   - `ASPNETCORE_ENVIRONMENT=Production`
   - Connection strings
   - JWT settings

### Frontend (Vercel / Netlify)

1. **Build the application:**

   ```bash
   npm run build
   ```

2. **Configure environment variables:**
   - `VITE_API_URL=https://your-backend-url.com/api`

3. **Deploy `dist` folder**

---

## 🔒 Security Checklist

Before deploying to production:

- [ ] Change JWT secret to a strong random string
- [ ] Enable HTTPS
- [ ] Update CORS to allow only production domain
- [ ] Use environment variables for secrets
- [ ] Enable rate limiting
- [ ] Set up proper logging
- [ ] Configure database backups
- [ ] Review and test all endpoints

---

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [React Documentation](https://react.dev/)
- [Vite Documentation](https://vitejs.dev/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [JWT Authentication](https://jwt.io/introduction)

---

## 💡 Tips

1. **Use migrations** for database changes in production
2. **Test API endpoints** with Swagger before frontend integration
3. **Check browser console** for frontend errors
4. **Use SQL Server Profiler** to debug database queries
5. **Keep dependencies updated** regularly

---

## 🆘 Need Help?

If you encounter issues:

1. Check the error messages in:
   - Visual Studio Output window
   - Browser console (F12)
   - Terminal output

2. Verify all services are running:
   - Database server
   - Backend API
   - Frontend dev server

3. Review the setup steps above

4. Check the API documentation in `docs/API.md`

---

## ✅ Quick Start Checklist

- [ ] Install all prerequisites
- [ ] Clone/download the project
- [ ] Configure database connection string
- [ ] Update JWT secret
- [ ] Run database migrations
- [ ] Install frontend dependencies
- [ ] Start backend server
- [ ] Start frontend dev server
- [ ] Test with Swagger
- [ ] Create test user and group
- [ ] Add sample expenses
- [ ] Verify all features work

---

**You're all set! 🎉** Start building and tracking your shared expenses!
