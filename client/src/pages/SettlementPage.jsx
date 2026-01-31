import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  groupService,
  expenseService,
  balanceService,
} from "../services/apiService";
import { calculateSettlements, formatCurrency } from "../utils/helpers";
import { exportGroupToPDF } from "../utils/pdfExport";

function SettlementPage({ user }) {
  const { groupId } = useParams();
  const navigate = useNavigate();
  const [group, setGroup] = useState(null);
  const [expenses, setExpenses] = useState([]);
  const [balances, setBalances] = useState([]);
  const [settlements, setSettlements] = useState([]);
  const [paidSettlements, setPaidSettlements] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchData();
  }, [groupId]);

  const fetchData = async () => {
    try {
      const [groupData, expensesData, balancesData, settlementsData] =
        await Promise.all([
          groupService.getGroup(groupId),
          expenseService.getExpenses(groupId),
          balanceService.getBalances(groupId),
          balanceService.getSettlements(groupId),
        ]);

      setGroup(groupData);
      setExpenses(expensesData);
      setBalances(balancesData);
      setPaidSettlements(settlementsData);

      // Calculate optimized settlements
      const optimized = calculateSettlements(balancesData);
      setSettlements(optimized);
    } catch (error) {
      console.error("Failed to fetch data:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleMarkAsPaid = async (settlement) => {
    try {
      await balanceService.createSettlement(groupId, {
        fromUserId: settlement.from,
        toUserId: settlement.to,
        amount: settlement.amount,
      });

      alert("Settlement marked as paid!");
      fetchData();
    } catch (error) {
      alert("Failed to mark settlement as paid");
    }
  };

  const handleExportPDF = () => {
    exportGroupToPDF(group, expenses, settlements, balances);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-xl text-gray-600">Loading...</div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Header */}
      <div className="mb-8">
        <button
          onClick={() => navigate(`/group/${groupId}`)}
          className="text-primary-600 hover:text-primary-700 mb-4"
        >
          ← Back to Group
        </button>

        <div className="flex justify-between items-start">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Settlements</h1>
            <p className="text-gray-600 mt-2">{group?.groupName}</p>
          </div>

          <button onClick={handleExportPDF} className="btn btn-primary">
            📄 Export PDF
          </button>
        </div>
      </div>

      {/* Optimized Settlements */}
      <div className="card mb-8">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-2xl font-semibold text-gray-900">
            Optimized Settlements
          </h2>
          <span className="text-sm text-gray-600">
            {settlements.length} transaction
            {settlements.length !== 1 ? "s" : ""} needed
          </span>
        </div>

        {settlements.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-gray-600 text-lg">
              🎉 All settled up! No pending payments.
            </p>
          </div>
        ) : (
          <div className="space-y-4">
            {settlements.map((settlement, index) => (
              <div
                key={index}
                className="flex items-center justify-between p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition"
              >
                <div className="flex items-center space-x-4">
                  <div className="flex items-center space-x-2">
                    <div className="w-10 h-10 bg-red-100 text-red-600 rounded-full flex items-center justify-center font-semibold">
                      {settlement.fromName.charAt(0).toUpperCase()}
                    </div>
                    <div>
                      <p className="font-semibold text-gray-900">
                        {settlement.fromName}
                      </p>
                      <p className="text-sm text-gray-600">Owes</p>
                    </div>
                  </div>

                  <div className="flex items-center space-x-2">
                    <span className="text-2xl">→</span>
                  </div>

                  <div className="flex items-center space-x-2">
                    <div className="w-10 h-10 bg-green-100 text-green-600 rounded-full flex items-center justify-center font-semibold">
                      {settlement.toName.charAt(0).toUpperCase()}
                    </div>
                    <div>
                      <p className="font-semibold text-gray-900">
                        {settlement.toName}
                      </p>
                      <p className="text-sm text-gray-600">Receives</p>
                    </div>
                  </div>
                </div>

                <div className="flex items-center space-x-4">
                  <div className="text-right">
                    <p className="text-2xl font-bold text-primary-600">
                      {formatCurrency(settlement.amount)}
                    </p>
                  </div>

                  {user?.userId === settlement.from && (
                    <button
                      onClick={() => handleMarkAsPaid(settlement)}
                      className="btn btn-primary text-sm"
                    >
                      Mark as Paid
                    </button>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Payment History */}
      {paidSettlements.length > 0 && (
        <div className="card">
          <h2 className="text-2xl font-semibold text-gray-900 mb-6">
            Payment History
          </h2>
          <div className="space-y-3">
            {paidSettlements.map((settlement) => (
              <div
                key={settlement.settlementId}
                className="flex items-center justify-between py-3 border-b border-gray-200 last:border-0"
              >
                <div className="flex-1">
                  <p className="text-gray-900">
                    <span className="font-semibold">
                      {settlement.fromUserName}
                    </span>
                    {" paid "}
                    <span className="font-semibold">
                      {settlement.toUserName}
                    </span>
                  </p>
                  <p className="text-sm text-gray-600">
                    {new Date(settlement.createdAt).toLocaleDateString(
                      "en-IN",
                      {
                        year: "numeric",
                        month: "short",
                        day: "numeric",
                        hour: "2-digit",
                        minute: "2-digit",
                      },
                    )}
                  </p>
                  {settlement.note && (
                    <p className="text-sm text-gray-600 mt-1">
                      {settlement.note}
                    </p>
                  )}
                </div>
                <div className="text-right">
                  <p className="font-bold text-green-600">
                    {formatCurrency(settlement.amount)}
                  </p>
                  <p className="text-xs text-gray-500">Paid</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Balance Summary */}
      <div className="mt-8 card">
        <h2 className="text-2xl font-semibold text-gray-900 mb-6">
          Current Balances
        </h2>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {balances.map((balance) => (
            <div
              key={balance.userId}
              className={`p-4 rounded-lg ${
                balance.balance > 0
                  ? "bg-green-50 border border-green-200"
                  : balance.balance < 0
                    ? "bg-red-50 border border-red-200"
                    : "bg-gray-50 border border-gray-200"
              }`}
            >
              <p className="font-semibold text-gray-900 mb-2">
                {balance.userName}
              </p>
              <p className="text-sm text-gray-600">
                Paid: {formatCurrency(balance.totalPaid)}
              </p>
              <p className="text-sm text-gray-600">
                Share: {formatCurrency(balance.totalShare)}
              </p>
              <div className="mt-2 pt-2 border-t border-gray-300">
                <p
                  className={`font-bold ${
                    balance.balance > 0
                      ? "text-green-600"
                      : balance.balance < 0
                        ? "text-red-600"
                        : "text-gray-600"
                  }`}
                >
                  {balance.balance > 0
                    ? `Gets back ${formatCurrency(balance.balance)}`
                    : balance.balance < 0
                      ? `Owes ${formatCurrency(Math.abs(balance.balance))}`
                      : "Settled up"}
                </p>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default SettlementPage;
