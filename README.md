# 💰 Expense Splitter

A full-stack expense splitting web application with optimized settlement algorithms, JWT authentication, and PDF reporting.

## 🎯 Features

- **Group Management**: Create groups for trips, roommates, or hostels
- **Expense Tracking**: Add and track shared expenses
- **Smart Splitting**: Equal, unequal, and percentage-based splits
- **Settlement Optimization**: Minimum transaction algorithm (Splitwise-style)
- **PDF Export**: Download detailed expense reports
- **Role-Based Access**: Admin and member roles with different permissions

## 🏗️ Tech Stack

### Frontend

- React + Vite
- Tailwind CSS
- Chart.js (balance visualization)
- jsPDF (PDF export)
- Axios (API communication)

### Backend

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- JWT Authentication
- SQL Server / PostgreSQL

### Database

- SQL Server (or PostgreSQL)
- Optimized schema with proper relationships

## 📁 Project Structure

```
expense-splitter/
│
├── client/              # React frontend
│   ├── src/
│   │   ├── components/  # Reusable UI components
│   │   ├── pages/       # Page components
│   │   ├── services/    # API services
│   │   ├── utils/       # Helper functions
│   │   └── App.jsx      # Main app component
│   └── package.json
│
├── server/              # ASP.NET Core backend
│   ├── Controllers/     # API controllers
│   ├── Models/          # Database models
│   ├── Services/        # Business logic
│   ├── DTOs/            # Data transfer objects
│   └── Program.cs       # Entry point
│
├── database/            # Database scripts
│   └── schema.sql       # Database schema
│
└── docs/                # Documentation
    └── API.md           # API documentation
```

## 🚀 Getting Started

### Prerequisites

- Node.js (v18+)
- .NET 8 SDK
- SQL Server or PostgreSQL
- Visual Studio 2022 (for backend)

### Frontend Setup

```bash
cd client
npm install
npm run dev
```

### Backend Setup

1. Open `server/ExpenseSplitter.sln` in Visual Studio
2. Update connection string in `appsettings.json`
3. Run migrations: `dotnet ef database update`
4. Press F5 to run

## 🧮 Settlement Algorithm

The app uses an optimized debt settlement algorithm that:

1. Calculates each user's net balance (paid - owed)
2. Separates creditors (positive balance) and debtors (negative balance)
3. Matches highest creditor with highest debtor
4. Minimizes the number of transactions needed

This is the same algorithm used by Splitwise and other expense apps.

## 📊 Database Schema

- **Users**: User authentication and profiles
- **Groups**: Expense groups
- **GroupMembers**: User-group relationships
- **Expenses**: Individual expenses
- **ExpenseSplits**: How expenses are divided
- **Settlements**: Payment records

## 🔐 Security

- JWT token-based authentication
- Password hashing with bcrypt
- Role-based authorization
- Group-level access control
- Input validation on all endpoints

## 📝 API Documentation

See [API.md](docs/API.md) for complete API documentation.

## 🎨 Screenshots

(Add screenshots here after implementation)

## 👨‍💻 Author

Built as a full-stack portfolio project demonstrating:

- Complex algorithm implementation
- RESTful API design
- Modern frontend development
- Database design and optimization
- Authentication and authorization

## 📄 License

MIT License
