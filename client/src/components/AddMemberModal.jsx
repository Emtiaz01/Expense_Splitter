import { useState } from "react";
import { groupService } from "../services/apiService";

function AddMemberModal({ isOpen, onClose, groupId, onMemberAdded }) {
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");
    setLoading(true);

    try {
      // Search for user by email and add them
      const result = await groupService.searchAndAddMember(groupId, email);
      setSuccess(`${email} has been added to the group!`);
      setEmail("");

      // Wait a moment to show success message
      setTimeout(() => {
        onMemberAdded();
        onClose();
        setSuccess("");
      }, 1500);
    } catch (err) {
      setError(
        err.response?.data?.message ||
          "Failed to add member. Make sure the email is registered.",
      );
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md">
        <h2 className="text-2xl font-bold mb-4">Add Member</h2>

        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label className="block text-gray-700 text-sm font-bold mb-2">
              Member Email *
            </label>
            <input
              type="email"
              className="input"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Enter member's email address"
              required
            />
            <p className="text-xs text-gray-500 mt-1">
              The user must be registered on the platform
            </p>
          </div>

          {error && (
            <div className="mb-4 p-3 bg-red-100 text-red-700 rounded">
              {error}
            </div>
          )}

          {success && (
            <div className="mb-4 p-3 bg-green-100 text-green-700 rounded">
              {success}
            </div>
          )}

          <div className="flex justify-end space-x-3">
            <button
              type="button"
              onClick={() => {
                onClose();
                setEmail("");
                setError("");
                setSuccess("");
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
              {loading ? "Adding..." : "Add Member"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default AddMemberModal;
