import { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { invitationService } from "../services/apiService";

function AcceptInvitation({ user, onLogin }) {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [invitation, setInvitation] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [accepting, setAccepting] = useState(false);

  const token = searchParams.get("token");

  useEffect(() => {
    verifyInvitation();
  }, [token]);

  const verifyInvitation = async () => {
    if (!token) {
      setError("Invalid invitation link");
      setLoading(false);
      return;
    }

    try {
      const data = await invitationService.verifyInvitation(token);
      setInvitation(data);
    } catch (err) {
      setError("Invitation not found or has expired");
    } finally {
      setLoading(false);
    }
  };

  const handleAcceptInvitation = async () => {
    if (!user) {
      // Store token in localStorage to process after login/register
      localStorage.setItem("pendingInvitation", token);
      navigate(
        `/register?email=${encodeURIComponent(invitation.email)}&invitation=${token}`,
      );
      return;
    }

    // User is logged in, accept invitation
    setAccepting(true);
    try {
      await invitationService.acceptInvitation(token);
      navigate(`/group/${invitation.groupId}`);
    } catch (err) {
      setError(err.response?.data?.message || "Failed to accept invitation");
    } finally {
      setAccepting(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-gray-50">
        <div className="text-center">
          <div className="w-16 h-16 border-4 border-blue-200 border-t-blue-600 rounded-full animate-spin mx-auto mb-4"></div>
          <p className="text-gray-600">Verifying invitation...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-gray-50">
        <div className="card max-w-md text-center">
          <div className="w-16 h-16 bg-red-100 text-red-600 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg
              className="w-8 h-8"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </div>
          <h2 className="text-2xl font-bold text-gray-900 mb-2">
            Invalid Invitation
          </h2>
          <p className="text-gray-600 mb-6">{error}</p>
          <button
            onClick={() => navigate("/login")}
            className="btn btn-primary"
          >
            Go to Login
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-50 py-12 px-4">
      <div className="card max-w-2xl w-full">
        <div className="text-center mb-8">
          <div className="w-20 h-20 bg-blue-100 text-blue-600 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg
              className="w-10 h-10"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
              />
            </svg>
          </div>
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            You're Invited!
          </h1>
          <p className="text-gray-600">
            Join the group and start tracking expenses together
          </p>
        </div>

        <div className="bg-blue-50 rounded-lg p-6 mb-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-2">
            Invitation Details
          </h2>
          <div className="space-y-2 text-sm">
            <div className="flex justify-between">
              <span className="text-gray-600">Group:</span>
              <span className="font-semibold text-gray-900">
                {invitation.groupName}
              </span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Invited by:</span>
              <span className="font-semibold text-gray-900">
                {invitation.invitedByName}
              </span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Your email:</span>
              <span className="font-semibold text-gray-900">
                {invitation.email}
              </span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Expires:</span>
              <span className="font-semibold text-gray-900">
                {new Date(invitation.expiresAt).toLocaleDateString()}
              </span>
            </div>
          </div>
        </div>

        {!user ? (
          <div className="space-y-4">
            <p className="text-center text-gray-600">
              To accept this invitation, you need to have an account
            </p>
            <div className="flex flex-col sm:flex-row gap-3">
              <button
                onClick={() =>
                  navigate(
                    `/register?email=${encodeURIComponent(invitation.email)}&invitation=${token}`,
                  )
                }
                className="flex-1 btn btn-primary"
              >
                Create Account
              </button>
              <button
                onClick={() => {
                  localStorage.setItem("pendingInvitation", token);
                  navigate("/login");
                }}
                className="flex-1 btn btn-secondary"
              >
                I Have an Account
              </button>
            </div>
          </div>
        ) : (
          <div className="space-y-4">
            <p className="text-center text-gray-600">
              You're logged in as <strong>{user.email}</strong>
            </p>
            <button
              onClick={handleAcceptInvitation}
              className="w-full btn btn-primary"
              disabled={accepting}
            >
              {accepting ? "Accepting..." : "Accept Invitation & Join Group"}
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

export default AcceptInvitation;
