# 📚 Expense Splitter API Documentation

Base URL: `http://localhost:5000/api`

## 🔐 Authentication

All endpoints except `/auth/register` and `/auth/login` require JWT authentication.

Include the JWT token in the Authorization header:

```
Authorization: Bearer <your_jwt_token>
```

---

## 📋 Table of Contents

1. [Authentication](#authentication)
2. [Groups](#groups)
3. [Expenses](#expenses)
4. [Balances & Settlements](#balances--settlements)

---

## Authentication

### Register User

**POST** `/auth/register`

Register a new user account.

**Request Body:**

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "password123"
}
```

**Response:** `200 OK`

```json
{
  "user": {
    "userId": 1,
    "name": "John Doe",
    "email": "john@example.com"
  },
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### Login

**POST** `/auth/login`

Authenticate and get JWT token.

**Request Body:**

```json
{
  "email": "john@example.com",
  "password": "password123"
}
```

**Response:** `200 OK`

```json
{
  "user": {
    "userId": 1,
    "name": "John Doe",
    "email": "john@example.com"
  },
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### Get Current User

**GET** `/auth/me`

Get currently authenticated user details.

**Headers:**

```
Authorization: Bearer <token>
```

**Response:** `200 OK`

```json
{
  "userId": 1,
  "name": "John Doe",
  "email": "john@example.com"
}
```

---

## Groups

### Get All User Groups

**GET** `/groups`

Get all groups the authenticated user is a member of.

**Response:** `200 OK`

```json
[
  {
    "groupId": 1,
    "groupName": "Trip to Goa",
    "description": "Beach vacation expenses",
    "createdBy": 1,
    "creatorName": "John Doe",
    "createdAt": "2026-01-15T10:00:00Z",
    "isClosed": false,
    "memberCount": 3,
    "expenseCount": 5,
    "totalExpenses": 15000.0,
    "userBalance": -500.0,
    "members": []
  }
]
```

---

### Get Group by ID

**GET** `/groups/{id}`

Get detailed information about a specific group.

**Response:** `200 OK`

```json
{
  "groupId": 1,
  "groupName": "Trip to Goa",
  "description": "Beach vacation expenses",
  "createdBy": 1,
  "creatorName": "John Doe",
  "createdAt": "2026-01-15T10:00:00Z",
  "isClosed": false,
  "memberCount": 3,
  "expenseCount": 5,
  "totalExpenses": 15000.0,
  "members": [
    {
      "groupMemberId": 1,
      "userId": 1,
      "name": "John Doe",
      "email": "john@example.com",
      "role": "Admin",
      "joinedAt": "2026-01-15T10:00:00Z"
    },
    {
      "groupMemberId": 2,
      "userId": 2,
      "name": "Jane Smith",
      "email": "jane@example.com",
      "role": "Member",
      "joinedAt": "2026-01-16T12:00:00Z"
    }
  ]
}
```

---

### Create Group

**POST** `/groups`

Create a new group. The creator automatically becomes an admin member.

**Request Body:**

```json
{
  "groupName": "Trip to Goa",
  "description": "Beach vacation expenses"
}
```

**Response:** `201 Created`

```json
{
  "groupId": 1,
  "groupName": "Trip to Goa",
  "description": "Beach vacation expenses",
  "createdBy": 1,
  "creatorName": "John Doe",
  "createdAt": "2026-01-15T10:00:00Z",
  "isClosed": false,
  "memberCount": 1,
  "expenseCount": 0,
  "totalExpenses": 0
}
```

---

### Add Member to Group

**POST** `/groups/{id}/members`

Add a user to a group. Only admins can add members.

**Request Body:**

```json
{
  "userId": 2
}
```

**Response:** `200 OK`

```json
{
  "message": "Member added successfully"
}
```

---

### Remove Member from Group

**DELETE** `/groups/{id}/members/{memberId}`

Remove a member from a group. Only admins can remove other members.

**Response:** `200 OK`

```json
{
  "message": "Member removed successfully"
}
```

---

### Close Group

**PUT** `/groups/{id}/close`

Close a group (final settlement). Only admins can close groups.

**Response:** `200 OK`

```json
{
  "message": "Group closed successfully"
}
```

---

## Expenses

### Get Group Expenses

**GET** `/groups/{groupId}/expenses`

Get all expenses for a specific group.

**Response:** `200 OK`

```json
[
  {
    "expenseId": 1,
    "groupId": 1,
    "amount": 3000.0,
    "paidByUserId": 1,
    "paidByUserName": "John Doe",
    "description": "Hotel booking",
    "splitType": "Equal",
    "createdAt": "2026-01-16T15:00:00Z",
    "splits": [
      {
        "splitId": 1,
        "userId": 1,
        "userName": "John Doe",
        "shareAmount": 1000.0,
        "percentage": null
      },
      {
        "splitId": 2,
        "userId": 2,
        "userName": "Jane Smith",
        "shareAmount": 1000.0,
        "percentage": null
      },
      {
        "splitId": 3,
        "userId": 3,
        "userName": "Bob Wilson",
        "shareAmount": 1000.0,
        "percentage": null
      }
    ]
  }
]
```

---

### Create Expense

**POST** `/expenses`

Create a new expense.

**Request Body (Equal Split):**

```json
{
  "groupId": 1,
  "amount": 3000.0,
  "paidByUserId": 1,
  "description": "Hotel booking",
  "splitType": "Equal",
  "splits": [
    { "userId": 1, "shareAmount": 1000.0 },
    { "userId": 2, "shareAmount": 1000.0 },
    { "userId": 3, "shareAmount": 1000.0 }
  ]
}
```

**Request Body (Unequal Split):**

```json
{
  "groupId": 1,
  "amount": 1000.0,
  "paidByUserId": 1,
  "description": "Dinner",
  "splitType": "Unequal",
  "splits": [
    { "userId": 1, "shareAmount": 500.0 },
    { "userId": 2, "shareAmount": 300.0 },
    { "userId": 3, "shareAmount": 200.0 }
  ]
}
```

**Request Body (Percentage Split):**

```json
{
  "groupId": 1,
  "amount": 1000.0,
  "paidByUserId": 1,
  "description": "Groceries",
  "splitType": "Percentage",
  "splits": [
    { "userId": 1, "shareAmount": 500.0, "percentage": 50.0 },
    { "userId": 2, "shareAmount": 300.0, "percentage": 30.0 },
    { "userId": 3, "shareAmount": 200.0, "percentage": 20.0 }
  ]
}
```

**Response:** `201 Created`

```json
{
  "expenseId": 1,
  "groupId": 1,
  "amount": 3000.00,
  "paidByUserId": 1,
  "paidByUserName": "John Doe",
  "description": "Hotel booking",
  "splitType": "Equal",
  "createdAt": "2026-01-16T15:00:00Z",
  "splits": [...]
}
```

---

### Update Expense

**PUT** `/expenses/{id}`

Update an existing expense. Only admins can update expenses.

**Request Body:**

```json
{
  "amount": 3500.0,
  "description": "Hotel booking (updated)",
  "splitType": "Equal",
  "splits": [
    { "userId": 1, "shareAmount": 1166.67 },
    { "userId": 2, "shareAmount": 1166.67 },
    { "userId": 3, "shareAmount": 1166.66 }
  ]
}
```

**Response:** `200 OK`

---

### Delete Expense

**DELETE** `/expenses/{id}`

Delete an expense. Only admins can delete expenses.

**Response:** `200 OK`

```json
{
  "message": "Expense deleted successfully"
}
```

---

## Balances & Settlements

### Get Group Balances

**GET** `/groups/{groupId}/balances`

Get balance summary for all members in a group.

**Response:** `200 OK`

```json
[
  {
    "userId": 1,
    "userName": "John Doe",
    "totalPaid": 5000.0,
    "totalShare": 4500.0,
    "balance": 500.0
  },
  {
    "userId": 2,
    "userName": "Jane Smith",
    "totalPaid": 3000.0,
    "totalShare": 4500.0,
    "balance": -1500.0
  },
  {
    "userId": 3,
    "userName": "Bob Wilson",
    "totalPaid": 2000.0,
    "totalShare": 4500.0,
    "balance": -2500.0
  }
]
```

**Balance Explanation:**

- `totalPaid`: Total amount paid by the user
- `totalShare`: Total amount user should pay (their share)
- `balance`: `totalPaid - totalShare`
  - Positive = Gets back money
  - Negative = Owes money
  - Zero = Settled up

---

### Get Settlements

**GET** `/groups/{groupId}/settlements`

Get all recorded settlements (payments) for a group.

**Response:** `200 OK`

```json
[
  {
    "settlementId": 1,
    "fromUserId": 2,
    "fromUserName": "Jane Smith",
    "toUserId": 1,
    "toUserName": "John Doe",
    "amount": 500.0,
    "createdAt": "2026-01-20T10:00:00Z",
    "note": "Paid via UPI"
  }
]
```

---

### Record Settlement

**POST** `/groups/{groupId}/settle`

Record a settlement (payment) between two users.

**Request Body:**

```json
{
  "fromUserId": 2,
  "toUserId": 1,
  "amount": 500.0,
  "note": "Paid via UPI"
}
```

**Response:** `200 OK`

```json
{
  "settlementId": 1,
  "fromUserId": 2,
  "fromUserName": "Jane Smith",
  "toUserId": 1,
  "toUserName": "John Doe",
  "amount": 500.0,
  "createdAt": "2026-01-20T10:00:00Z",
  "note": "Paid via UPI"
}
```

---

## Error Responses

### 400 Bad Request

```json
{
  "message": "Validation error message"
}
```

### 401 Unauthorized

```json
{
  "message": "Invalid email or password"
}
```

### 403 Forbidden

```json
{
  "message": "Only admins can perform this action"
}
```

### 404 Not Found

```json
{
  "message": "Resource not found"
}
```

---

## 🧮 Settlement Algorithm

The frontend implements an optimized settlement algorithm that:

1. Calculates each user's net balance (paid - owed)
2. Separates creditors (positive balance) and debtors (negative balance)
3. Matches highest creditor with highest debtor
4. Minimizes the number of transactions

**Example:**

- User A: Paid ₹5000, Owes ₹3000 → Balance: +₹2000 (gets back)
- User B: Paid ₹1000, Owes ₹3000 → Balance: -₹2000 (owes)
- User C: Paid ₹4000, Owes ₹4000 → Balance: ₹0 (settled)

**Optimized Settlement:** B pays A ₹2000 (1 transaction instead of multiple)

---

## Testing with Swagger

The API includes Swagger UI for testing:

1. Start the backend server
2. Navigate to: `http://localhost:5000/swagger`
3. Click "Authorize" and enter: `Bearer <your_token>`
4. Test all endpoints interactively

---

## Rate Limiting & Security

- Password hashing: BCrypt
- JWT expiry: 24 hours (configurable)
- HTTPS recommended in production
- CORS configured for localhost development
- SQL injection protection via EF Core parameterized queries

---

## Database Migrations

```bash
# Create migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName
```

---

## Support

For issues or questions, refer to the main [README.md](../README.md).
