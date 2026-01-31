-- ============================================
-- Expense Splitter Database Schema
-- Database: SQL Server / PostgreSQL
-- ============================================

-- Users Table
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Groups Table
CREATE TABLE Groups (
    GroupId INT PRIMARY KEY IDENTITY(1,1),
    GroupName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    IsClosed BIT DEFAULT 0,
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
);

-- GroupMembers Table
CREATE TABLE GroupMembers (
    GroupMemberId INT PRIMARY KEY IDENTITY(1,1),
    GroupId INT NOT NULL,
    UserId INT NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'Member')),
    JoinedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (GroupId) REFERENCES Groups(GroupId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    UNIQUE(GroupId, UserId)
);

-- Expenses Table
CREATE TABLE Expenses (
    ExpenseId INT PRIMARY KEY IDENTITY(1,1),
    GroupId INT NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL CHECK (Amount > 0),
    PaidByUserId INT NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    SplitType NVARCHAR(20) NOT NULL CHECK (SplitType IN ('Equal', 'Unequal', 'Percentage')),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (GroupId) REFERENCES Groups(GroupId) ON DELETE CASCADE,
    FOREIGN KEY (PaidByUserId) REFERENCES Users(UserId)
);

-- ExpenseSplits Table
CREATE TABLE ExpenseSplits (
    SplitId INT PRIMARY KEY IDENTITY(1,1),
    ExpenseId INT NOT NULL,
    UserId INT NOT NULL,
    ShareAmount DECIMAL(18, 2) NOT NULL CHECK (ShareAmount >= 0),
    Percentage DECIMAL(5, 2),
    FOREIGN KEY (ExpenseId) REFERENCES Expenses(ExpenseId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Settlements Table
CREATE TABLE Settlements (
    SettlementId INT PRIMARY KEY IDENTITY(1,1),
    FromUserId INT NOT NULL,
    ToUserId INT NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL CHECK (Amount > 0),
    GroupId INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    Note NVARCHAR(500),
    FOREIGN KEY (FromUserId) REFERENCES Users(UserId),
    FOREIGN KEY (ToUserId) REFERENCES Users(UserId),
    FOREIGN KEY (GroupId) REFERENCES Groups(GroupId) ON DELETE CASCADE
);

-- Indexes for Performance
CREATE INDEX IX_GroupMembers_GroupId ON GroupMembers(GroupId);
CREATE INDEX IX_GroupMembers_UserId ON GroupMembers(UserId);
CREATE INDEX IX_Expenses_GroupId ON Expenses(GroupId);
CREATE INDEX IX_Expenses_PaidByUserId ON Expenses(PaidByUserId);
CREATE INDEX IX_ExpenseSplits_ExpenseId ON ExpenseSplits(ExpenseId);
CREATE INDEX IX_ExpenseSplits_UserId ON ExpenseSplits(UserId);
CREATE INDEX IX_Settlements_GroupId ON Settlements(GroupId);
CREATE INDEX IX_Settlements_FromUserId ON Settlements(FromUserId);
CREATE INDEX IX_Settlements_ToUserId ON Settlements(ToUserId);

-- Sample Data (Optional - for testing)
-- INSERT INTO Users (Name, Email, PasswordHash) VALUES 
-- ('Alice', 'alice@example.com', 'hashed_password_here'),
-- ('Bob', 'bob@example.com', 'hashed_password_here'),
-- ('Charlie', 'charlie@example.com', 'hashed_password_here');
