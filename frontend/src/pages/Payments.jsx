import { useState, useEffect } from "react";
import { paymentAPI, policyAPI } from "../services/api";
import { useAuth } from "../context/AuthContext";
import Layout from "../components/Layout";
import LoadingSpinner from "../components/LoadingSpinner";
import ErrorAlert from "../components/ErrorAlert";
import {
  FiPlus,
  FiCreditCard,
  FiCalendar,
  FiDollarSign,
  FiCheckCircle,
  FiXCircle,
} from "react-icons/fi";

const Payments = () => {
  const { user: _user } = useAuth();
  const [payments, setPayments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [showRecordModal, setShowRecordModal] = useState(false);

  useEffect(() => {
    fetchPayments();
  }, []);

  const fetchPayments = async () => {
    try {
      setLoading(true);
      const response = await paymentAPI.getAll();
      setPayments(response.data);
    } catch (err) {
      setError(err.response?.data?.message || "Failed to load payments");
    } finally {
      setLoading(false);
    }
  };

  const getMethodIcon = () => {
    return <FiCreditCard className="text-gray-600" />;
  };

  const getStatusColor = (status) => {
    const colors = {
      Completed: "bg-green-100 text-green-700",
      Pending: "bg-yellow-100 text-yellow-700",
      Failed: "bg-red-100 text-red-700",
      Refunded: "bg-gray-100 text-gray-700",
    };
    return colors[status] || "bg-gray-100 text-gray-700";
  };

  const getStatusIcon = (status) => {
    switch (status) {
      case "Completed":
        return <FiCheckCircle className="text-green-600" />;
      case "Failed":
        return <FiXCircle className="text-red-600" />;
      default:
        return <FiCreditCard className="text-gray-600" />;
    }
  };

  const totalPaid = payments
    .filter((p) => p.status === "Completed")
    .reduce((sum, p) => sum + p.amount, 0);

  if (loading) {
    return (
      <Layout>
        <LoadingSpinner message="Loading payments..." />
      </Layout>
    );
  }

  return (
    <Layout>
      <div>
        {/* Header */}
        <div className="flex justify-between items-center mb-6">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">
              Payment History
            </h1>
            <p className="text-gray-600 mt-1">Track your insurance payments</p>
          </div>
          <button
            onClick={() => setShowRecordModal(true)}
            className="btn btn-primary flex items-center gap-2"
          >
            <FiPlus />
            Record Payment
          </button>
        </div>

        <ErrorAlert message={error} onClose={() => setError("")} />

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div className="card">
            <p className="text-sm text-gray-600 mb-1">Total Payments</p>
            <p className="text-2xl font-bold text-gray-900">
              {payments.length}
            </p>
          </div>
          <div className="card">
            <p className="text-sm text-gray-600 mb-1">Total Paid</p>
            <p className="text-2xl font-bold text-green-600">
              R{totalPaid.toLocaleString()}
            </p>
          </div>
          <div className="card">
            <p className="text-sm text-gray-600 mb-1">Completed</p>
            <p className="text-2xl font-bold text-green-600">
              {payments.filter((p) => p.status === "Completed").length}
            </p>
          </div>
          <div className="card">
            <p className="text-sm text-gray-600 mb-1">Pending</p>
            <p className="text-2xl font-bold text-yellow-600">
              {payments.filter((p) => p.status === "Pending").length}
            </p>
          </div>
        </div>

        {/* Payments List */}
        {payments.length === 0 ? (
          <div className="card text-center py-12">
            <FiCreditCard className="text-6xl text-gray-300 mx-auto mb-4" />
            <h3 className="text-xl font-semibold text-gray-900 mb-2">
              No payments yet
            </h3>
            <p className="text-gray-600 mb-6">
              Record your first payment to get started
            </p>
            <button
              onClick={() => setShowRecordModal(true)}
              className="btn btn-primary inline-flex items-center gap-2"
            >
              <FiPlus />
              Record Payment
            </button>
          </div>
        ) : (
          <div className="card overflow-x-auto">
            <table className="w-full">
              <thead className="border-b border-gray-200">
                <tr className="text-left">
                  <th className="pb-3 text-sm font-semibold text-gray-700">
                    Transaction ID
                  </th>
                  <th className="pb-3 text-sm font-semibold text-gray-700">
                    Policy
                  </th>
                  <th className="pb-3 text-sm font-semibold text-gray-700">
                    Amount
                  </th>
                  <th className="pb-3 text-sm font-semibold text-gray-700">
                    Method
                  </th>
                  <th className="pb-3 text-sm font-semibold text-gray-700">
                    Date
                  </th>
                  <th className="pb-3 text-sm font-semibold text-gray-700">
                    Status
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {payments.map((payment) => (
                  <tr key={payment.id} className="hover:bg-gray-50">
                    <td className="py-4">
                      <div className="flex items-center gap-2">
                        {getStatusIcon(payment.status)}
                        <span className="font-medium text-gray-900">
                          {payment.transactionId}
                        </span>
                      </div>
                    </td>
                    <td className="py-4 text-sm text-gray-600">
                      {payment.policyNumber}
                    </td>
                    <td className="py-4">
                      <span className="font-semibold text-gray-900">
                        R{payment.amount.toLocaleString()}
                      </span>
                    </td>
                    <td className="py-4">
                      <div className="flex items-center gap-2 text-sm text-gray-600">
                        {getMethodIcon(payment.method)}
                        <span>{payment.method}</span>
                      </div>
                    </td>
                    <td className="py-4 text-sm text-gray-600">
                      {new Date(payment.paymentDate).toLocaleDateString()}
                    </td>
                    <td className="py-4">
                      <span
                        className={`inline-block px-3 py-1 rounded-full text-xs font-medium ${getStatusColor(
                          payment.status
                        )}`}
                      >
                        {payment.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {/* Record Payment Modal */}
        {showRecordModal && (
          <RecordPaymentModal
            onClose={() => setShowRecordModal(false)}
            onSuccess={() => {
              setShowRecordModal(false);
              fetchPayments();
            }}
          />
        )}
      </div>
    </Layout>
  );
};

// Record Payment Modal Component
const RecordPaymentModal = ({ onClose, onSuccess }) => {
  const [policies, setPolicies] = useState([]);
  const [formData, setFormData] = useState({
    policyId: "",
    amount: "",
    method: 1, // Default to CreditCard
    reference: "",
  });
  const [loading, setLoading] = useState(false);
  const [loadingPolicies, setLoadingPolicies] = useState(true);
  const [error, setError] = useState("");

  const paymentMethods = [
    { value: 1, label: "Credit Card", icon: "💳" },
    { value: 2, label: "Debit Card", icon: "💳" },
    { value: 3, label: "Bank Transfer", icon: "🏦" },
    { value: 4, label: "Cash", icon: "💵" },
    { value: 5, label: "Mobile Payment", icon: "📱" },
  ];

  useEffect(() => {
    fetchPolicies();
  }, []);

  const fetchPolicies = async () => {
    try {
      const response = await policyAPI.getAll();
      const activePolicies = response.data.filter((p) => p.status === "Active");
      setPolicies(activePolicies);
    } catch (error) {
      setError("Failed to load policies");
      console.error("Error loading policies:", error);
    } finally {
      setLoadingPolicies(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      await paymentAPI.record({
        ...formData,
        policyId: parseInt(formData.policyId),
        amount: parseFloat(formData.amount),
        method: parseInt(formData.method),
        reference: formData.reference || null,
      });
      onSuccess();
    } catch (err) {
      setError(err.response?.data?.message || "Failed to record payment");
    } finally {
      setLoading(false);
    }
  };

  const selectedPolicy = policies.find(
    (p) => p.id === parseInt(formData.policyId)
  );

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          <h2 className="text-2xl font-bold text-gray-900 mb-4">
            Record Payment
          </h2>

          <ErrorAlert message={error} onClose={() => setError("")} />

          {loadingPolicies ? (
            <LoadingSpinner size="sm" message="Loading policies..." />
          ) : policies.length === 0 ? (
            <div className="text-center py-8">
              <FiCreditCard className="text-4xl text-gray-300 mx-auto mb-3" />
              <p className="text-gray-600">No active policies available</p>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-4">
              {/* Policy Selection */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Select Policy
                </label>
                <select
                  required
                  className="input"
                  value={formData.policyId}
                  onChange={(e) =>
                    setFormData({ ...formData, policyId: e.target.value })
                  }
                >
                  <option value="">Choose a policy...</option>
                  {policies.map((policy) => (
                    <option key={policy.id} value={policy.id}>
                      {policy.policyNumber} - {policy.type} (Premium: R
                      {policy.premiumAmount.toLocaleString()}/month)
                    </option>
                  ))}
                </select>
                {selectedPolicy && (
                  <p className="text-xs text-gray-500 mt-1">
                    Suggested amount: R
                    {selectedPolicy.premiumAmount.toLocaleString()} (monthly
                    premium)
                  </p>
                )}
              </div>

              {/* Amount */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Amount (R)
                </label>
                <input
                  type="number"
                  required
                  min="1"
                  max="10000000"
                  step="0.01"
                  className="input"
                  placeholder="3800"
                  value={formData.amount}
                  onChange={(e) =>
                    setFormData({ ...formData, amount: e.target.value })
                  }
                />
              </div>

              {/* Payment Method */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Payment Method
                </label>
                <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
                  {paymentMethods.map((method) => (
                    <label
                      key={method.value}
                      className={`cursor-pointer border-2 rounded-lg p-3 transition ${
                        formData.method === method.value
                          ? "border-primary-600 bg-primary-50"
                          : "border-gray-200 hover:border-gray-300"
                      }`}
                    >
                      <input
                        type="radio"
                        name="method"
                        value={method.value}
                        checked={formData.method === method.value}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            method: parseInt(e.target.value),
                          })
                        }
                        className="sr-only"
                      />
                      <div className="text-center">
                        <div className="text-2xl mb-1">{method.icon}</div>
                        <div className="text-xs font-medium text-gray-900">
                          {method.label}
                        </div>
                      </div>
                    </label>
                  ))}
                </div>
              </div>

              {/* Reference */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Reference (Optional)
                </label>
                <input
                  type="text"
                  className="input"
                  placeholder="External reference or transaction ID"
                  value={formData.reference}
                  onChange={(e) =>
                    setFormData({ ...formData, reference: e.target.value })
                  }
                  maxLength="200"
                />
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
                  {loading ? "Recording..." : "Record Payment"}
                </button>
              </div>
            </form>
          )}
        </div>
      </div>
    </div>
  );
};

export default Payments;
