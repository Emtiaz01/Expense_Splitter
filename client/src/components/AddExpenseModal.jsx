import { useState } from "react";
import { expenseService } from "../services/apiService";
import { calculateSplits, validateSplits } from "../utils/helpers";

function AddExpenseModal({
  isOpen,
  onClose,
  groupId,
  members,
  onExpenseAdded,
  user,
}) {
  const [formData, setFormData] = useState({
    amount: "",
    description: "",
    paidByUserId: user?.userId || "",
    splitType: "Equal",
    participants: [],
  });
  const [customSplits, setCustomSplits] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      const amount = Number(formData.amount);

      // Calculate splits
      let splits = calculateSplits(
        amount,
        formData.participants,
        formData.splitType,
        customSplits,
      );

      // Validate splits
      const validation = validateSplits(amount, splits, formData.splitType);
      if (!validation.valid) {
        setError(validation.message);
        setLoading(false);
        return;
      }

      const expenseData = {
        groupId: Number(groupId),
        amount,
        description: formData.description,
        paidByUserId: Number(formData.paidByUserId),
        splitType: formData.splitType,
        splits,
      };

      await expenseService.createExpense(expenseData);
      onExpenseAdded();
      resetForm();
      onClose();
    } catch (err) {
      setError(err.response?.data?.message || "Failed to add expense");
    } finally {
      setLoading(false);
    }
  };

  const resetForm = () => {
    setFormData({
      amount: "",
      description: "",
      paidByUserId: user?.userId || "",
      splitType: "Equal",
      participants: [],
    });
    setCustomSplits([]);
  };

  const handleParticipantToggle = (userId) => {
    setFormData((prev) => ({
      ...prev,
      participants: prev.participants.includes(userId)
        ? prev.participants.filter((id) => id !== userId)
        : [...prev.participants, userId],
    }));
  };

  const handleCustomSplitChange = (userId, field, value) => {
    setCustomSplits((prev) => {
      const existing = prev.find((s) => s.userId === userId);
      if (existing) {
        return prev.map((s) =>
          s.userId === userId ? { ...s, [field]: Number(value) } : s,
        );
      } else {
        return [...prev, { userId, [field]: Number(value) }];
      }
    });
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 overflow-y-auto">
      <div className="bg-white rounded-lg p-8 max-w-2xl w-full mx-4 my-8">
        <h2 className="text-2xl font-bold mb-4">Add Expense</h2>

        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="grid grid-cols-2 gap-4 mb-4">
            <div>
              <label className="block text-gray-700 text-sm font-bold mb-2">
                Amount (৳) *
              </label>
              <input
                type="number"
                step="0.01"
                className="input"
                value={formData.amount}
                onChange={(e) =>
                  setFormData({ ...formData, amount: e.target.value })
                }
                placeholder="0.00"
                required
              />
            </div>

            <div>
              <label className="block text-gray-700 text-sm font-bold mb-2">
                Paid By *
              </label>
              <select
                className="input"
                value={formData.paidByUserId}
                onChange={(e) =>
                  setFormData({ ...formData, paidByUserId: e.target.value })
                }
                required
              >
                <option value="">Select member</option>
                {members.map((member) => (
                  <option key={member.userId} value={member.userId}>
                    {member.name}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="mb-4">
            <label className="block text-gray-700 text-sm font-bold mb-2">
              Description *
            </label>
            <input
              type="text"
              className="input"
              value={formData.description}
              onChange={(e) =>
                setFormData({ ...formData, description: e.target.value })
              }
              placeholder="Dinner, Taxi, Hotel, etc."
              required
            />
          </div>

          <div className="mb-4">
            <label className="block text-gray-700 text-sm font-bold mb-2">
              Split Type *
            </label>
            <select
              className="input"
              value={formData.splitType}
              onChange={(e) =>
                setFormData({ ...formData, splitType: e.target.value })
              }
              required
            >
              <option value="Equal">Equal Split</option>
              <option value="Unequal">Unequal Split</option>
              <option value="Percentage">Percentage Split</option>
            </select>
          </div>

          <div className="mb-4">
            <label className="block text-gray-700 text-sm font-bold mb-2">
              Participants * (Select who shares this expense)
            </label>
            <div className="space-y-2">
              {members.map((member) => (
                <div
                  key={member.userId}
                  className="flex items-center space-x-2"
                >
                  <input
                    type="checkbox"
                    id={`participant-${member.userId}`}
                    checked={formData.participants.includes(member.userId)}
                    onChange={() => handleParticipantToggle(member.userId)}
                    className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                  />
                  <label
                    htmlFor={`participant-${member.userId}`}
                    className="flex-1"
                  >
                    {member.name}
                  </label>

                  {formData.participants.includes(member.userId) &&
                    formData.splitType === "Unequal" && (
                      <input
                        type="number"
                        step="0.01"
                        placeholder="Amount"
                        className="input w-32"
                        onChange={(e) =>
                          handleCustomSplitChange(
                            member.userId,
                            "shareAmount",
                            e.target.value,
                          )
                        }
                      />
                    )}

                  {formData.participants.includes(member.userId) &&
                    formData.splitType === "Percentage" && (
                      <input
                        type="number"
                        step="0.01"
                        placeholder="%"
                        className="input w-24"
                        onChange={(e) =>
                          handleCustomSplitChange(
                            member.userId,
                            "percentage",
                            e.target.value,
                          )
                        }
                      />
                    )}
                </div>
              ))}
            </div>
          </div>

          <div className="flex justify-end space-x-3 mt-6">
            <button
              type="button"
              onClick={() => {
                resetForm();
                onClose();
              }}
              className="btn btn-secondary"
              disabled={loading}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="btn btn-primary"
              disabled={loading}
            >
              {loading ? "Adding..." : "Add Expense"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default AddExpenseModal;
