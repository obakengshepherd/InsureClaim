import { useState, useEffect } from "react";
import { claimAPI, policyAPI } from "../services/api";
import { useAuth } from "../context/AuthContext";
import Layout from "../components/Layout";
import LoadingSpinner from "../components/LoadingSpinner";
import ErrorAlert from "../components/ErrorAlert";
import {
  FiPlus,
  FiAlertCircle,
  FiCalendar,
  FiDollarSign,
  FiFileText,
  FiClock,
  FiSearch,
} from "react-icons/fi";
import toast from "react-hot-toast";

const Claims = () => {
  const { user: _user } = useAuth();
  const [claims, setClaims] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [showSubmitModal, setShowSubmitModal] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");

  useEffect(() => {
    fetchClaims();
  }, []);

  const fetchClaims = async () => {
    try {
      setLoading(true);
      const response = await claimAPI.getAll();
      setClaims(response.data);
    } catch (error) {
      const message = error.response?.data?.message || "Failed to load claims";
      setError(message);
      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status) => {
    const colors = {
      Submitted: "bg-blue-100 text-blue-700",
      UnderReview: "bg-yellow-100 text-yellow-700",
      Approved: "bg-green-100 text-green-700",
      Denied: "bg-red-100 text-red-700",
      Paid: "bg-purple-100 text-purple-700",
    };
    return colors[status] || "bg-gray-100 text-gray-700";
  };

  const getStatusIcon = (status) => {
    switch (status) {
      case "Submitted":
        return <FiClock className="text-blue-600" />;
      case "UnderReview":
        return <FiFileText className="text-yellow-600" />;
      case "Approved":
        return <FiAlertCircle className="text-green-600" />;
      case "Denied":
        return <FiAlertCircle className="text-red-600" />;
      case "Paid":
        return <FiDollarSign className="text-purple-600" />;
      default:
        return <FiAlertCircle className="text-gray-600" />;
    }
  };

  const filteredClaims = claims.filter((claim) => {
    const query = searchQuery.toLowerCase();
    return (
      claim.claimNumber.toLowerCase().includes(query) ||
      claim.policyNumber.toLowerCase().includes(query) ||
      claim.description.toLowerCase().includes(query)
    );
  });

  if (loading) {
    return (
      <Layout>
        <LoadingSpinner message="Loading claims..." />
      </Layout>
    );
  }

  return (
    <Layout>
      <div>
        {/* Header */}
        <div className="flex justify-between items-center mb-6">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">My Claims</h1>
            <p className="text-gray-600 mt-1">Track your insurance claims</p>
          </div>
          <button
            onClick={() => setShowSubmitModal(true)}
            className="btn btn-primary flex items-center gap-2"
          >
            <FiPlus />
            Submit Claim
          </button>
        </div>

        <ErrorAlert message={error} onClose={() => setError("")} />

        {/* Search Bar */}
        {claims.length > 0 && (
          <div className="mb-6">
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <FiSearch className="text-gray-400" />
              </div>
              <input
                type="text"
                placeholder="Search claims by claim number, policy, or description..."
                className="input pl-10"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
            </div>
          </div>
        )}

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div className="card">
            <p className="text-sm text-gray-600 mb-1">Total Claims</p>
            <p className="text-2xl font-bold text-gray-900">{claims.length}</p>
          </div>
          <div className="card">
            <p className="text-sm text-gray-600 mb-1">Approved</p>
            <p className="text-2xl font-bold text-green-600">
              {claims.filter((c) => c.status === "Approved").length}
            </p>
          </div>
          <div className="card">
            <p className="text-sm text-gray-600 mb-1">Under Review</p>
            <p className="text-2xl font-bold text-yellow-600">
              {claims.filter((c) => c.status === "UnderReview").length}
            </p>
          </div>
          <div className="card">
            <p className="text-sm text-gray-600 mb-1">Denied</p>
            <p className="text-2xl font-bold text-red-600">
              {claims.filter((c) => c.status === "Denied").length}
            </p>
          </div>
        </div>

        {/* Claims List */}
        {filteredClaims.length === 0 ? (
          <div className="card text-center py-12">
            <FiAlertCircle className="text-6xl text-gray-300 mx-auto mb-4" />
            <h3 className="text-xl font-semibold text-gray-900 mb-2">
              No claims found
            </h3>
            <p className="text-gray-600 mb-6">
              Try adjusting your search or submit a new claim.
            </p>
            <button
              onClick={() => setShowSubmitModal(true)}
              className="btn btn-primary inline-flex items-center gap-2"
            >
              <FiPlus />
              Submit Claim
            </button>
          </div>
        ) : (
          <div className="space-y-4">
            {filteredClaims.map((claim) => (
              <div
                key={claim.id}
                className="card hover:shadow-lg transition-shadow"
              >
                <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                  {/* Claim Info */}
                  <div className="flex-1">
                    <div className="flex items-center gap-3 mb-2">
                      <div className="p-2 bg-gray-100 rounded-lg">
                        {getStatusIcon(claim.status)}
                      </div>
                      <div>
                        <h3 className="font-bold text-lg text-gray-900">
                          {claim.claimNumber}
                        </h3>
                        <p className="text-sm text-gray-500">
                          Policy: {claim.policyNumber}
                        </p>
                      </div>
                    </div>
                    <p className="text-sm text-gray-700 mb-2 line-clamp-2">
                      {claim.description}
                    </p>
                    <div className="flex flex-wrap gap-4 text-sm text-gray-600">
                      <div className="flex items-center gap-1">
                        <FiDollarSign className="text-gray-400" />
                        <span>
                          Claimed: R{claim.claimAmount.toLocaleString()}
                        </span>
                      </div>
                      {claim.approvedAmount && (
                        <div className="flex items-center gap-1">
                          <FiDollarSign className="text-green-600" />
                          <span className="text-green-600">
                            Approved: R{claim.approvedAmount.toLocaleString()}
                          </span>
                        </div>
                      )}
                      <div className="flex items-center gap-1">
                        <FiCalendar className="text-gray-400" />
                        <span>
                          {new Date(claim.submittedDate).toLocaleDateString()}
                        </span>
                      </div>
                    </div>
                    {claim.reviewNotes && (
                      <div className="mt-2 p-2 bg-gray-50 rounded text-sm text-gray-700">
                        <strong>Notes:</strong> {claim.reviewNotes}
                      </div>
                    )}
                  </div>

                  {/* Status Badge */}
                  <div className="flex flex-col items-end gap-2">
                    <span
                      className={`px-4 py-2 rounded-full text-sm font-medium ${getStatusColor(
                        claim.status
                      )}`}
                    >
                      {claim.status}
                    </span>
                    {claim.reviewedDate && (
                      <span className="text-xs text-gray-500">
                        Reviewed:{" "}
                        {new Date(claim.reviewedDate).toLocaleDateString()}
                      </span>
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Submit Claim Modal */}
        {showSubmitModal && (
          <SubmitClaimModal
            onClose={() => setShowSubmitModal(false)}
            onSuccess={() => {
              setShowSubmitModal(false);
              fetchClaims();
            }}
          />
        )}
      </div>
    </Layout>
  );
};

// Submit Claim Modal Component
const SubmitClaimModal = ({ onClose, onSuccess }) => {
  const [policies, setPolicies] = useState([]);
  const [formData, setFormData] = useState({
    policyId: "",
    description: "",
    claimAmount: "",
    incidentDate: "",
  });
  const [loading, setLoading] = useState(false);
  const [loadingPolicies, setLoadingPolicies] = useState(true);
  const [error, setError] = useState("");

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
      await claimAPI.submit({
        ...formData,
        policyId: parseInt(formData.policyId),
        claimAmount: parseFloat(formData.claimAmount),
        documentPath: null,
      });
      toast.success("Claim submitted successfully!");
      onSuccess();
    } catch (error) {
      const message = error.response?.data?.message || "Failed to submit claim";
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
            Submit New Claim
          </h2>

          <ErrorAlert message={error} onClose={() => setError("")} />

          {loadingPolicies ? (
            <LoadingSpinner size="sm" message="Loading policies..." />
          ) : policies.length === 0 ? (
            <div className="text-center py-8">
              <FiAlertCircle className="text-4xl text-gray-300 mx-auto mb-3" />
              <p className="text-gray-600">No active policies available</p>
              <p className="text-sm text-gray-500 mt-1">
                You need an active policy to submit a claim
              </p>
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
                      {policy.policyNumber} - {policy.type} (Coverage: R
                      {policy.coverageAmount.toLocaleString()})
                    </option>
                  ))}
                </select>
              </div>

              {/* Description */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Description
                </label>
                <textarea
                  required
                  rows="4"
                  className="input"
                  placeholder="Describe what happened and the damage/loss..."
                  value={formData.description}
                  onChange={(e) =>
                    setFormData({ ...formData, description: e.target.value })
                  }
                  minLength="10"
                  maxLength="1000"
                />
                <p className="text-xs text-gray-500 mt-1">
                  {formData.description.length}/1000 characters (minimum 10)
                </p>
              </div>

              {/* Claim Amount */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Claim Amount (R)
                </label>
                <input
                  type="number"
                  required
                  min="100"
                  max="100000000"
                  step="0.01"
                  className="input"
                  placeholder="15000"
                  value={formData.claimAmount}
                  onChange={(e) =>
                    setFormData({ ...formData, claimAmount: e.target.value })
                  }
                />
                <p className="text-xs text-gray-500 mt-1">
                  Amount must not exceed policy coverage
                </p>
              </div>

              {/* Incident Date */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Incident Date
                </label>
                <input
                  type="date"
                  required
                  className="input"
                  value={formData.incidentDate}
                  onChange={(e) =>
                    setFormData({ ...formData, incidentDate: e.target.value })
                  }
                  max={new Date().toISOString().split("T")[0]}
                />
                <p className="text-xs text-gray-500 mt-1">
                  Must be within policy coverage period
                </p>
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
                  {loading ? "Submitting..." : "Submit Claim"}
                </button>
              </div>
            </form>
          )}
        </div>
      </div>
    </div>
  );
};

export default Claims;
