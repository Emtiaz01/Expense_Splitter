import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  groupService,
  expenseService,
  balanceService,
} from "../services/apiService";
import AddExpenseModal from "../components/AddExpenseModal";
import AddMemberModal from "../components/AddMemberModal";
import BalanceChart from "../components/BalanceChart";
import { formatCurrency, formatDate } from "../utils/helpers";

function GroupPage({ user }) {
  const { groupId } = useParams();
  const navigate = useNavigate();
  const [group, setGroup] = useState(null);
  const [expenses, setExpenses] = useState([]);
  const [balances, setBalances] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showAddExpense, setShowAddExpense] = useState(false);
  const [showAddMember, setShowAddMember] = useState(false);
  const [activeTab, setActiveTab] = useState("expenses"); // expenses, balances

  useEffect(() => {
    fetchGroupData();
  }, [groupId]);

  const fetchGroupData = async () => {
    try {
      const [groupData, expensesData, balancesData] = await Promise.all([
        groupService.getGroup(groupId),
        expenseService.getExpenses(groupId),
        balanceService.getBalances(groupId),
      ]);

      setGroup(groupData);
      setExpenses(expensesData);
      setBalances(balancesData);
    } catch (error) {
      console.error("Failed to fetch group data:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleExpenseAdded = () => {
    fetchGroupData();
  };

  const handleMemberAdded = () => {
    fetchGroupData();
  };

  const handleDeleteExpense = async (expenseId) => {
    if (window.confirm("Are you sure you want to delete this expense?")) {
      try {
        await expenseService.deleteExpense(expenseId);
        fetchGroupData();
      } catch (error) {
        alert("Failed to delete expense");
      }
    }
  };

  const isAdmin = group?.members?.some(
    (m) => m.userId === user?.userId && m.role === "Admin",
  );

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-xl text-gray-600">Loading...</div>
      </div>
    );
  }

  if (!group) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-xl text-red-600">Group not found</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Back Button & Header */}
        <div className="mb-8">
          <button
            onClick={() => navigate("/dashboard")}
            className="flex items-center gap-2 text-blue-600 hover:text-blue-700 mb-6 font-medium transition-colors"
          >
            <span>←</span> Back to Dashboard
          </button>

          <div className="card">
            <div className="flex flex-col lg:flex-row lg:justify-between lg:items-start gap-6">
              <div className="flex-1">
                <div className="flex items-center gap-3 mb-3">
                  <h1 className="text-3xl font-bold text-gray-900">
                    {group.groupName}
                  </h1>
                  {group.isClosed && (
                    <span className="badge badge-danger">Closed</span>
                  )}
                </div>

                {group.description && (
                  <p className="text-gray-600 mb-4">{group.description}</p>
                )}

                {/* Stats */}
                <div className="flex flex-wrap gap-4 text-sm">
                  <div className="flex items-center gap-2">
                    <span className="text-gray-600">Members:</span>
                    <span className="font-semibold text-gray-900">
                      {group.members?.length || 0}
                    </span>
                  </div>

                  <div className="flex items-center gap-2">
                    <span className="text-gray-600">Expenses:</span>
                    <span className="font-semibold text-gray-900">
                      {expenses.length}
                    </span>
                  </div>

                  <div className="flex items-center gap-2">
                    <span className="text-gray-600">Total:</span>
                    <span className="font-semibold text-blue-600">
                      {formatCurrency(
                        expenses.reduce((sum, e) => sum + e.amount, 0),
                      )}
                    </span>
                  </div>
                </div>
              </div>

              {/* Action Buttons */}
              <div className="flex flex-col sm:flex-row gap-3">
                <button
                  onClick={() => navigate(`/group/${groupId}/settlements`)}
                  className="btn btn-secondary flex items-center justify-center gap-2"
                >
                  View Settlements
                </button>
                <button
                  onClick={() => setShowAddExpense(true)}
                  className="btn btn-primary flex items-center justify-center gap-2"
                  disabled={group.isClosed}
                >
                  <span>+</span>
                  Add Expense
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Tabs */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 mb-6 overflow-hidden">
          <nav className="flex border-b border-gray-200">
            <button
              onClick={() => setActiveTab("expenses")}
              className={`flex-1 py-4 px-6 font-semibold text-sm transition-colors ${
                activeTab === "expenses"
                  ? "text-blue-600 border-b-2 border-blue-600 bg-blue-50"
                  : "text-gray-600 hover:bg-gray-50"
              }`}
            >
              Expenses
            </button>
            <button
              onClick={() => setActiveTab("balances")}
              className={`flex-1 py-4 px-6 font-semibold text-sm transition-colors ${
                activeTab === "balances"
                  ? "text-blue-600 border-b-2 border-blue-600 bg-blue-50"
                  : "text-gray-600 hover:bg-gray-50"
              }`}
            >
              Balances
            </button>
            <button
              onClick={() => setActiveTab("members")}
              className={`flex-1 py-4 px-6 font-semibold text-sm transition-colors ${
                activeTab === "members"
                  ? "text-blue-600 border-b-2 border-blue-600 bg-blue-50"
                  : "text-gray-600 hover:bg-gray-50"
              }`}
            >
              Members
            </button>
          </nav>
        </div>

        {/* Content */}
        {activeTab === "expenses" && (
          <div className="space-y-4">
            {expenses.length === 0 ? (
              <div className="card text-center py-12">
                <div className="inline-flex items-center justify-center w-16 h-16 bg-gray-100 rounded-full mb-4">
                  <svg
                    className="w-8 h-8 text-gray-400"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                  </svg>
                </div>
                <p className="text-gray-600 font-medium">
                  No expenses yet. Add your first expense!
                </p>
              </div>
            ) : (
              expenses.map((expense, index) => (
                <div key={expense.expenseId} className="card">
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <div className="flex items-center space-x-3">
                        <h3 className="text-lg font-semibold text-gray-900">
                          {expense.description}
                        </h3>
                        <span className="px-2 py-1 text-xs font-semibold text-primary-600 bg-primary-100 rounded">
                          {expense.splitType}
                        </span>
                      </div>
                      <div className="mt-2 space-y-1 text-sm text-gray-600">
                        <p>
                          Paid by{" "}
                          <span className="font-semibold">
                            {expense.paidByUserName}
                          </span>
                        </p>
                        <p>{formatDate(expense.createdAt)}</p>
                        {expense.splits && (
                          <div className="mt-2">
                            <p className="font-semibold text-gray-700">
                              Split among:
                            </p>
                            <ul className="ml-4 mt-1">
                              {expense.splits.map((split, idx) => (
                                <li key={idx}>
                                  {split.userName}:{" "}
                                  {formatCurrency(split.shareAmount)}
                                  {split.percentage &&
                                    ` (${split.percentage}%)`}
                                </li>
                              ))}
                            </ul>
                          </div>
                        )}
                      </div>
                    </div>
                    <div className="text-right">
                      <div className="text-2xl font-bold text-gray-900">
                        {formatCurrency(expense.amount)}
                      </div>
                      {isAdmin && (
                        <button
                          onClick={() => handleDeleteExpense(expense.expenseId)}
                          className="mt-2 text-sm text-red-600 hover:text-red-700"
                        >
                          Delete
                        </button>
                      )}
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>
        )}

        {activeTab === "balances" && (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div className="card">
              <h2 className="text-xl font-semibold mb-4">Balance Summary</h2>
              <div className="space-y-3">
                {balances.map((balance) => (
                  <div
                    key={balance.userId}
                    className="flex justify-between items-center py-3 border-b border-gray-200 last:border-0"
                  >
                    <div>
                      <p className="font-semibold text-gray-900">
                        {balance.userName}
                      </p>
                      <p className="text-sm text-gray-600">
                        Paid: {formatCurrency(balance.totalPaid)} | Share:{" "}
                        {formatCurrency(balance.totalShare)}
                      </p>
                    </div>
                    <div className="text-right">
                      <p
                        className={`font-bold ${
                          balance.balance > 0
                            ? "text-green-600"
                            : balance.balance < 0
                              ? "text-red-600"
                              : "text-gray-600"
                        }`}
                      >
                        {formatCurrency(Math.abs(balance.balance))}
                      </p>
                      <p className="text-sm text-gray-600">
                        {balance.balance > 0
                          ? "gets back"
                          : balance.balance < 0
                            ? "owes"
                            : "settled"}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            <div className="card">
              <h2 className="text-xl font-semibold mb-4">Visual Balance</h2>
              <BalanceChart balances={balances} />
            </div>
          </div>
        )}

        {activeTab === "members" && (
          <div className="card">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-semibold">Group Members</h2>
              {isAdmin && (
                <button
                  onClick={() => setShowAddMember(true)}
                  className="btn btn-primary"
                  disabled={group.isClosed}
                >
                  + Add Member
                </button>
              )}
            </div>
            <div className="space-y-3">
              {group.members?.map((member) => (
                <div
                  key={member.userId}
                  className="flex justify-between items-center py-3 border-b border-gray-200 last:border-0"
                >
                  <div>
                    <p className="font-semibold text-gray-900">{member.name}</p>
                    <p className="text-sm text-gray-600">{member.email}</p>
                  </div>
                  <div className="flex items-center space-x-3">
                    <span
                      className={`px-2 py-1 text-xs font-semibold rounded ${
                        member.role === "Admin"
                          ? "bg-primary-100 text-primary-600"
                          : "bg-gray-100 text-gray-600"
                      }`}
                    >
                      {member.role}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        <AddExpenseModal
          isOpen={showAddExpense}
          onClose={() => setShowAddExpense(false)}
          groupId={groupId}
          members={group.members || []}
          onExpenseAdded={handleExpenseAdded}
          user={user}
        />

        <AddMemberModal
          isOpen={showAddMember}
          onClose={() => setShowAddMember(false)}
          groupId={groupId}
          onMemberAdded={handleMemberAdded}
        />
      </div>
    </div>
  );
}

export default GroupPage;
