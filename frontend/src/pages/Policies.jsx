import { useState, useEffect } from "react";
import { policyAPI } from "../services/api";
import { useAuth } from "../context/AuthContext";
import Layout from "../components/Layout";
import LoadingSpinner from "../components/LoadingSpinner";
import ErrorAlert from "../components/ErrorAlert";
import {
  FiPlus,
  FiFileText,
  FiCalendar,
  FiDollarSign,
  FiEye,
  FiSearch,
} from "react-icons/fi";
import toast from "react-hot-toast";

const Policies = () => {
  const { user: _user } = useAuth();
  const [policies, setPolicies] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [searchQuery, setSearchQuery] = useState(""); // ✅ Added search state

  useEffect(() => {
    fetchPolicies();
  }, []);

  const fetchPolicies = async () => {
    try {
      setLoading(true);
      const response = await policyAPI.getAll();
      setPolicies(response.data);
    } catch (error) {
      const message =
        error.response?.data?.message || "Failed to load policies";
      setError(message);
      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  // ✅ Filter logic for search bar
  const filteredPolicies = policies.filter((policy) => {
    const query = searchQuery.toLowerCase();
    return (
      policy.policyNumber.toLowerCase().includes(query) ||
      policy.type.toLowerCase().includes(query)
    );
  });

  const getPolicyTypeColor = (type) => {
    const colors = {
      Life: "bg-purple-100 text-purple-700",
      Auto: "bg-blue-100 text-blue-700",
      Health: "bg-green-100 text-green-700",
      Property: "bg-orange-100 text-orange-700",
    };
    return colors[type] || "bg-gray-100 text-gray-700";
  };

  const getStatusColor = (status) => {
    const colors = {
      Active: "bg-green-100 text-green-700",
      Expired: "bg-red-100 text-red-700",
      Cancelled: "bg-gray-100 text-gray-700",
      Suspended: "bg-yellow-100 text-yellow-700",
    };
    return colors[status] || "bg-gray-100 text-gray-700";
  };

  if (loading) {
    return (
      <Layout>
        <LoadingSpinner message="Loading policies..." />
      </Layout>
    );
  }

  return (
    <Layout>
      <div>
        {/* Header */}
        <div className="flex justify-between items-center mb-6">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">My Policies</h1>
            <p className="text-gray-600 mt-1">Manage your insurance policies</p>
          </div>
          <button
            onClick={() => setShowCreateModal(true)}
            className="btn btn-primary flex items-center gap-2"
          >
            <FiPlus />
            New Policy
          </button>
        </div>

        {/* Error message */}
        <ErrorAlert message={error} onClose={() => setError("")} />

        {/* ✅ Search Bar */}
        {policies.length > 0 && (
          <div className="mb-6">
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <FiSearch className="text-gray-400" />
              </div>
              <input
                type="text"
                placeholder="Search policies by policy number or type..."
                className="input pl-10"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
            </div>
          </div>
        )}

        {/* ✅ Policies Grid */}
        {filteredPolicies.length === 0 ? (
          <div className="card text-center py-12">
            <FiFileText className="text-6xl text-gray-300 mx-auto mb-4" />
            <h3 className="text-xl font-semibold text-gray-900 mb-2">
              {policies.length === 0
                ? "No policies yet"
                : "No policies match your search"}
            </h3>
            <p className="text-gray-600 mb-6">
              {policies.length === 0
                ? "Get started by creating your first insurance policy"
                : "Try adjusting your search terms"}
            </p>
            <button
              onClick={() => setShowCreateModal(true)}
              className="btn btn-primary inline-flex items-center gap-2"
            >
              <FiPlus />
              Create Policy
            </button>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {filteredPolicies.map((policy) => (
              <div
                key={policy.id}
                className="card hover:shadow-lg transition-shadow cursor-pointer"
              >
                {/* Policy Header */}
                <div className="flex justify-between items-start mb-4">
                  <div>
                    <h3 className="font-bold text-lg text-gray-900">
                      {policy.policyNumber}
                    </h3>
                    <span
                      className={`inline-block px-2 py-1 rounded-full text-xs font-medium mt-1 ${getPolicyTypeColor(
                        policy.type
                      )}`}
                    >
                      {policy.type}
                    </span>
                  </div>
                  <span
                    className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(
                      policy.status
                    )}`}
                  >
                    {policy.status}
                  </span>
                </div>

                {/* Policy Details */}
                <div className="space-y-3">
                  <div className="flex items-center gap-2 text-sm">
                    <FiDollarSign className="text-gray-400" />
                    <span className="text-gray-600">Coverage:</span>
                    <span className="font-semibold text-gray-900">
                      R{policy.coverageAmount.toLocaleString()}
                    </span>
                  </div>
                  <div className="flex items-center gap-2 text-sm">
                    <FiDollarSign className="text-gray-400" />
                    <span className="text-gray-600">Premium:</span>
                    <span className="font-semibold text-gray-900">
                      R{policy.premiumAmount.toLocaleString()}/month
                    </span>
                  </div>
                  <div className="flex items-center gap-2 text-sm">
                    <FiCalendar className="text-gray-400" />
                    <span className="text-gray-600">Valid until:</span>
                    <span className="font-semibold text-gray-900">
                      {new Date(policy.endDate).toLocaleDateString()}
                    </span>
                  </div>
                </div>

                {/* Actions */}
                <div className="mt-4 pt-4 border-t border-gray-200">
                  <button className="w-full btn btn-secondary text-sm flex items-center justify-center gap-2">
                    <FiEye />
                    View Details
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Create Policy Modal */}
        {showCreateModal && (
          <CreatePolicyModal
            onClose={() => setShowCreateModal(false)}
            onSuccess={() => {
              setShowCreateModal(false);
              fetchPolicies();
            }}
          />
        )}
      </div>
    </Layout>
  );
};

// ✅ Create Policy Modal (unchanged)
const CreatePolicyModal = ({ onClose, onSuccess }) => {
  const { user } = useAuth();
  const [formData, setFormData] = useState({
    userId: user?.userId || 0,
    type: 2,
    coverageAmount: "",
    startDate: new Date().toISOString().split("T")[0],
    durationMonths: 12,
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const policyTypes = [
    { value: 1, label: "Life Insurance", description: "0.5% monthly premium" },
    { value: 2, label: "Auto Insurance", description: "0.8% monthly premium" },
    {
      value: 3,
      label: "Health Insurance",
      description: "0.6% monthly premium",
    },
    {
      value: 4,
      label: "Property Insurance",
      description: "0.4% monthly premium",
    },
  ];

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      await policyAPI.create({
        ...formData,
        coverageAmount: parseFloat(formData.coverageAmount),
      });
      toast.success("Policy created successfully!");
      onSuccess();
    } catch (error) {
      const message =
        error.response?.data?.message || "Failed to create policy";
      setError(message);
      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          <h2 className="text-2xl font-bold text-gray-900 mb-4">
            Create New Policy
          </h2>

          <ErrorAlert message={error} onClose={() => setError("")} />

          <form onSubmit={handleSubmit} className="space-y-4">
            {/* Policy Type */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Policy Type
              </label>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                {policyTypes.map((type) => (
                  <label
                    key={type.value}
                    className={`cursor-pointer border-2 rounded-lg p-4 transition ${
                      formData.type === type.value
                        ? "border-primary-600 bg-primary-50"
                        : "border-gray-200 hover:border-gray-300"
                    }`}
                  >
                    <input
                      type="radio"
                      name="type"
                      value={type.value}
                      checked={formData.type === type.value}
                      onChange={(e) =>
                        setFormData({
                          ...formData,
                          type: parseInt(e.target.value),
                        })
                      }
                      className="sr-only"
                    />
                    <div className="font-medium text-gray-900">
                      {type.label}
                    </div>
                    <div className="text-xs text-gray-500 mt-1">
                      {type.description}
                    </div>
                  </label>
                ))}
              </div>
            </div>

            {/* Coverage Amount */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Coverage Amount (R)
              </label>
              <input
                type="number"
                required
                min="1000"
                max="100000000"
                className="input"
                placeholder="500000"
                value={formData.coverageAmount}
                onChange={(e) =>
                  setFormData({ ...formData, coverageAmount: e.target.value })
                }
              />
              <p className="text-xs text-gray-500 mt-1">
                Minimum: R1,000 | Maximum: R100,000,000
              </p>
            </div>

            {/* Start Date */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Start Date
              </label>
              <input
                type="date"
                required
                className="input"
                value={formData.startDate}
                onChange={(e) =>
                  setFormData({ ...formData, startDate: e.target.value })
                }
              />
            </div>

            {/* Duration */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Duration (Months)
              </label>
              <select
                className="input"
                value={formData.durationMonths}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    durationMonths: parseInt(e.target.value),
                  })
                }
              >
                <option value="6">6 months</option>
                <option value="12">12 months (5% discount)</option>
                <option value="24">24 months (10% discount)</option>
                <option value="36">36 months (10% discount)</option>
              </select>
            </div>

            {/* Actions */}
            <div className="flex gap-3 pt-4">
              <button
                type="button"
                onClick={onClose}
                className="flex-1 btn btn-secondary"
                disabled={loading}
              >
                Cancel
              </button>
              <button
                type="submit"
                className="flex-1 btn btn-primary"
                disabled={loading}
              >
                {loading ? "Creating..." : "Create Policy"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default Policies;
