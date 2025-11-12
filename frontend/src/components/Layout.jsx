import { useAuth } from "../context/AuthContext";
import { Link, useLocation } from "react-router-dom";
import {
  FiShield,
  FiFileText,
  FiAlertCircle,
  FiCreditCard,
  FiBarChart2,
  FiLogOut,
  FiUser,
} from "react-icons/fi";

const Layout = ({ children }) => {
  const { user, logout, isAdmin } = useAuth();
  const location = useLocation();

  const navigation = [
    { name: "Dashboard", href: "/dashboard", icon: FiBarChart2 },
    { name: "Policies", href: "/policies", icon: FiFileText },
    { name: "Claims", href: "/claims", icon: FiAlertCircle },
    { name: "Payments", href: "/payments", icon: FiCreditCard },
    // Add admin link conditionally
    ...(isAdmin ? [{ name: "Admin", href: "/admin", icon: FiShield }] : []),
  ];

  const isActive = (href) => location.pathname === href;

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Navigation Bar */}
      <nav className="bg-white shadow-md sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            {/* Logo */}
            <Link
              to="/dashboard"
              className="flex items-center gap-2 hover:opacity-80 transition"
            >
              <div className="p-2 bg-primary-600 rounded-lg">
                <FiShield className="text-white text-xl" />
              </div>
              <span className="text-xl font-bold text-gray-900">
                InsureClaim
              </span>
            </Link>

            {/* Desktop Navigation */}
            <div className="hidden md:flex items-center gap-1">
              {navigation.map((item) => {
                const Icon = item.icon;
                return (
                  <Link
                    key={item.name}
                    to={item.href}
                    className={`flex items-center gap-2 px-4 py-2 rounded-lg transition ${
                      isActive(item.href)
                        ? "bg-primary-50 text-primary-700 font-medium"
                        : "text-gray-600 hover:bg-gray-100"
                    }`}
                  >
                    <Icon className="text-lg" />
                    <span>{item.name}</span>
                  </Link>
                );
              })}
            </div>

            {/* User Menu */}
            <div className="flex items-center gap-4">
              <Link
                to="/profile"
                className="hidden sm:block text-right hover:opacity-80 transition"
              >
                <p className="text-sm font-medium text-gray-900">
                  {user?.fullName}
                </p>
                <p className="text-xs text-gray-500">{user?.role}</p>
              </Link>
              <button
                onClick={logout}
                className="flex items-center gap-2 btn btn-secondary text-sm"
              >
                <FiLogOut />
                <span className="hidden sm:inline">Logout</span>
              </button>
            </div>
          </div>
        </div>

        {/* Mobile Navigation */}
        <div className="md:hidden border-t border-gray-200">
          <div className="flex justify-around py-2">
            {navigation.map((item) => {
              const Icon = item.icon;
              return (
                <Link
                  key={item.name}
                  to={item.href}
                  className={`flex flex-col items-center gap-1 px-3 py-2 rounded-lg transition ${
                    isActive(item.href) ? "text-primary-600" : "text-gray-600"
                  }`}
                >
                  <Icon className="text-xl" />
                  <span className="text-xs">{item.name}</span>
                </Link>
              );
            })}
          </div>
        </div>
      </nav>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {children}
      </main>
    </div>
  );
};

export default Layout;
