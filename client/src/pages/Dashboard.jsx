import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { groupService } from "../services/apiService";
import CreateGroupModal from "../components/CreateGroupModal";
import { formatCurrency, formatDate } from "../utils/helpers";

function Dashboard({ user }) {
  const [groups, setGroups] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    fetchGroups();
  }, []);

  const fetchGroups = async () => {
    try {
      const data = await groupService.getGroups();
      setGroups(data);
    } catch (error) {
      console.error("Failed to fetch groups:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleGroupCreated = (newGroup) => {
    setGroups([...groups, newGroup]);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="flex flex-col items-center">
          <div className="w-16 h-16 border-4 border-indigo-200 border-t-indigo-600 rounded-full animate-spin mb-4"></div>
          <p className="text-xl text-gray-600 font-medium">
            Loading your groups...
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-8">
          <div className="flex flex-col md:flex-row md:justify-between md:items-start gap-4">
            <div>
              <h1 className="page-header mb-2">Welcome back, {user?.name}</h1>
              <p className="text-gray-600">
                Manage and track your shared expenses
              </p>
            </div>
            <button
              onClick={() => setShowCreateModal(true)}
              className="btn btn-primary flex items-center gap-2"
            >
              <span className="text-lg">+</span>
              Create Group
            </button>
          </div>
        </div>

        {groups.length === 0 ? (
          <div className="card text-center py-16">
            <div className="max-w-md mx-auto">
              <div className="inline-flex items-center justify-center w-20 h-20 bg-blue-50 rounded-full mb-6 border-4 border-blue-100">
                <span className="text-4xl">📊</span>
              </div>
              <h2 className="text-2xl font-bold text-gray-900 mb-3">
                No groups yet
              </h2>
              <p className="text-gray-600 mb-6">
                Create a group to start tracking expenses with friends,
                roommates, or colleagues.
              </p>
              <button
                onClick={() => setShowCreateModal(true)}
                className="btn btn-primary"
              >
                Create Your First Group
              </button>
            </div>
          </div>
        ) : (
          <>
            {/* Stats Cards */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
              <div className="stat-card">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-gray-600 mb-1">Total Groups</p>
                    <p className="text-3xl font-bold text-gray-900">
                      {groups.length}
                    </p>
                  </div>
                  <div className="w-12 h-12 bg-blue-50 rounded-lg flex items-center justify-center">
                    <span className="text-2xl">📁</span>
                  </div>
                </div>
              </div>

              <div className="stat-card">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-gray-600 mb-1">Active Groups</p>
                    <p className="text-3xl font-bold text-gray-900">
                      {groups.filter((g) => !g.isClosed).length}
                    </p>
                  </div>
                  <div className="w-12 h-12 bg-green-50 rounded-lg flex items-center justify-center">
                    <span className="text-2xl">✓</span>
                  </div>
                </div>
              </div>

              <div className="stat-card">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-gray-600 mb-1">Total Expenses</p>
                    <p className="text-3xl font-bold text-gray-900">
                      {groups.reduce(
                        (sum, g) => sum + (g.expenseCount || 0),
                        0,
                      )}
                    </p>
                  </div>
                  <div className="w-12 h-12 bg-orange-50 rounded-lg flex items-center justify-center">
                    <span className="text-2xl">💵</span>
                  </div>
                </div>
              </div>
            </div>

            {/* Groups Grid */}
            <div>
              <h2 className="section-header mb-6">Your Groups</h2>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {groups.map((group) => (
                  <div
                    key={group.groupId}
                    onClick={() => navigate(`/group/${group.groupId}`)}
                    className="card-hover"
                  >
                    {/* Group Header */}
                    <div className="flex items-start justify-between mb-4">
                      <div className="flex-1">
                        <h3 className="text-lg font-bold text-gray-900 mb-2">
                          {group.groupName}
                        </h3>
                        {group.isClosed && (
                          <span className="badge badge-danger">Closed</span>
                        )}
                      </div>
                    </div>

                    {/* Description */}
                    {group.description && (
                      <p className="text-gray-600 text-sm mb-4 line-clamp-2">
                        {group.description}
                      </p>
                    )}

                    {/* Stats */}
                    <div className="space-y-2 mb-4">
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-gray-600">Members</span>
                        <span className="font-semibold text-gray-900">
                          {group.memberCount || 0}
                        </span>
                      </div>

                      <div className="flex items-center justify-between text-sm">
                        <span className="text-gray-600">Expenses</span>
                        <span className="font-semibold text-gray-900">
                          {group.expenseCount || 0}
                        </span>
                      </div>

                      <div className="flex items-center justify-between text-sm pt-2 border-t">
                        <span className="text-gray-600">Total</span>
                        <span className="font-bold text-blue-600">
                          {formatCurrency(group.totalExpenses || 0)}
                        </span>
                      </div>
                    </div>

                    {/* User Balance */}
                    {group.userBalance !== undefined &&
                      group.userBalance !== 0 && (
                        <div className="mt-4 pt-4 border-t border-gray-200">
                          <div className="flex justify-between items-center">
                            <span className="text-sm text-gray-600">
                              Your balance
                            </span>
                            <span
                              className={`font-semibold text-sm ${
                                group.userBalance > 0
                                  ? "text-green-600"
                                  : "text-red-600"
                              }`}
                            >
                              {group.userBalance > 0 ? "+" : ""}
                              {formatCurrency(group.userBalance)}
                            </span>
                          </div>
                        </div>
                      )}

                    {/* Created date */}
                    <div className="mt-4 pt-4 border-t border-gray-100">
                      <p className="text-xs text-gray-500">
                        Created {formatDate(group.createdAt)}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </>
        )}

        <CreateGroupModal
          isOpen={showCreateModal}
          onClose={() => setShowCreateModal(false)}
          onGroupCreated={handleGroupCreated}
        />
      </div>
    </div>
  );
}

export default Dashboard;
