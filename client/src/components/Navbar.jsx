import { Link } from "react-router-dom";

function Navbar({ user, onLogout }) {
  return (
    <nav className="bg-white border-b border-gray-200 sticky top-0 z-50 shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          {/* Logo Section */}
          <div className="flex items-center">
            <Link
              to="/dashboard"
              className="flex items-center gap-3 hover:opacity-80 transition-opacity"
            >
              <div className="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center shadow-sm">
                <span className="text-xl">💰</span>
              </div>
              <span className="text-xl font-bold text-gray-900">
                Expense Splitter
              </span>
            </Link>
          </div>

          {/* User Section */}
          <div className="flex items-center gap-4">
            <div className="hidden md:flex items-center gap-3 px-4 py-2 bg-gray-50 rounded-lg border border-gray-200">
              <div className="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center text-white font-semibold text-sm">
                {user?.name?.charAt(0).toUpperCase()}
              </div>
              <div className="text-left">
                <p className="text-sm font-semibold text-gray-900">
                  {user?.name}
                </p>
                <p className="text-xs text-gray-500">{user?.email}</p>
              </div>
            </div>

            <button onClick={onLogout} className="btn btn-secondary text-sm">
              Logout
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
}

export default Navbar;
