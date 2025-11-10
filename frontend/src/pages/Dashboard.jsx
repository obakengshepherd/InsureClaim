import { useAuth } from "../context/AuthContext";
import { FiUser, FiFileText, FiCreditCard, FiShield } from "react-icons/fi";

const Dashboard = () => {
  const { user, logout } = useAuth();

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Navigation Bar */}
      <nav className="bg-white shadow-md">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            {/* Logo */}
            <div className="flex items-center gap-2">
              <div className="p-2 bg-primary-600 rounded-lg">
                <FiShield className="text-white text-xl" />
              </div>
              <span className="text-xl font-bold text-gray-900">
                InsureClaim
              </span>
            </div>

            {/* User Menu */}
            <div className="flex items-center gap-4">
              <div className="text-right">
                <p className="text-sm font-medium text-gray-900">
                  {user?.fullName}
                </p>
                <p className="text-xs text-gray-500">{user?.role}</p>
              </div>
              <button onClick={logout} className="btn btn-secondary text-sm">
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>

      {/* Main Content */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Welcome Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">
            Welcome back, {user?.fullName}!
          </h1>
          <p className="text-gray-600 mt-1">
            Here's an overview of your insurance dashboard
          </p>
        </div>

        {/* Quick Stats */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          {/* Policies Card */}
          <div className="card hover:shadow-lg transition-shadow cursor-pointer">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600 mb-1">My Policies</p>
                <p className="text-3xl font-bold text-gray-900">0</p>
                <p className="text-xs text-gray-500 mt-1">Active policies</p>
              </div>
              <div className="p-4 bg-blue-50 rounded-full">
                <FiFileText className="text-3xl text-blue-600" />
              </div>
            </div>
          </div>

          {/* Claims Card */}
          <div className="card hover:shadow-lg transition-shadow cursor-pointer">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600 mb-1">My Claims</p>
                <p className="text-3xl font-bold text-gray-900">0</p>
                <p className="text-xs text-gray-500 mt-1">Total claims</p>
              </div>
              <div className="p-4 bg-green-50 rounded-full">
                <FiShield className="text-3xl text-green-600" />
              </div>
            </div>
          </div>

          {/* Payments Card */}
          <div className="card hover:shadow-lg transition-shadow cursor-pointer">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600 mb-1">Payments</p>
                <p className="text-3xl font-bold text-gray-900">R0</p>
                <p className="text-xs text-gray-500 mt-1">Total paid</p>
              </div>
              <div className="p-4 bg-purple-50 rounded-full">
                <FiCreditCard className="text-3xl text-purple-600" />
              </div>
            </div>
          </div>
        </div>

        {/* Quick Actions */}
        <div className="card">
          <h2 className="text-xl font-bold text-gray-900 mb-4">
            Quick Actions
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <button className="btn btn-primary py-4 text-left flex items-center justify-between">
              <span>View Policies</span>
              <FiFileText />
            </button>
            <button className="btn btn-primary py-4 text-left flex items-center justify-between">
              <span>Submit Claim</span>
              <FiShield />
            </button>
            <button className="btn btn-primary py-4 text-left flex items-center justify-between">
              <span>Payment History</span>
              <FiCreditCard />
            </button>
          </div>
        </div>

        {/* Coming Soon Message */}
        <div className="mt-8 card bg-primary-50 border border-primary-200">
          <div className="flex items-start gap-3">
            <div className="p-2 bg-primary-600 rounded-lg flex-shrink-0">
              <FiUser className="text-white" />
            </div>
            <div>
              <h3 className="font-semibold text-gray-900 mb-1">
                More features coming soon!
              </h3>
              <p className="text-sm text-gray-600">
                We're building policy management, claims submission, and payment
                tracking features. Stay tuned!
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
