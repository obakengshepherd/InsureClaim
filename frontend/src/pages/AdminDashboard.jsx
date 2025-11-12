import { useState, useEffect } from "react";
import { policyAPI, claimAPI, paymentAPI } from "../services/api";
import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";
import Layout from "../components/Layout";
import LoadingSpinner from "../components/LoadingSpinner";
import ErrorAlert from "../components/ErrorAlert";
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";
import {
  FiUsers,
  FiFileText,
  FiAlertCircle,
  FiDollarSign,
  FiTrendingUp,
} from "react-icons/fi";

const AdminDashboard = () => {
  const { isAdmin } = useAuth();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [stats, setStats] = useState(null);

  useEffect(() => {
    if (!isAdmin) {
      navigate("/dashboard");
      return;
    }
    fetchStatistics();
  }, [isAdmin, navigate]);

  const fetchStatistics = async () => {
    try {
      setLoading(true);
      const [
        policiesRes,
        claimsRes,
        paymentsRes,
        claimStatsRes,
        paymentStatsRes,
      ] = await Promise.all([
        policyAPI.getAll(),
        claimAPI.getAll(),
        paymentAPI.getAll(),
        claimAPI.getStatistics(),
        paymentAPI.getStatistics(),
      ]);

      const policies = policiesRes.data;
      const claims = claimsRes.data;
      const payments = paymentsRes.data;

      // Policy by type
      const policyByType = [
        {
          name: "Life",
          value: policies.filter((p) => p.type === "Life").length,
        },
        {
          name: "Auto",
          value: policies.filter((p) => p.type === "Auto").length,
        },
        {
          name: "Health",
          value: policies.filter((p) => p.type === "Health").length,
        },
        {
          name: "Property",
          value: policies.filter((p) => p.type === "Property").length,
        },
      ];

      // Policy by status
      const policyByStatus = [
        {
          name: "Active",
          value: policies.filter((p) => p.status === "Active").length,
        },
        {
          name: "Expired",
          value: policies.filter((p) => p.status === "Expired").length,
        },
        {
          name: "Cancelled",
          value: policies.filter((p) => p.status === "Cancelled").length,
        },
        {
          name: "Suspended",
          value: policies.filter((p) => p.status === "Suspended").length,
        },
      ];

      // Claims over time (group by month)
      const claimsByMonth = {};
      claims.forEach((claim) => {
        const month = new Date(claim.submittedDate).toLocaleDateString(
          "en-US",
          {
            year: "numeric",
            month: "short",
          }
        );
        claimsByMonth[month] = (claimsByMonth[month] || 0) + 1;
      });
      const claimsTimeline = Object.entries(claimsByMonth).map(
        ([month, count]) => ({
          month,
          claims: count,
        })
      );

      // Revenue over time
      const revenueByMonth = {};
      payments
        .filter((p) => p.status === "Completed")
        .forEach((payment) => {
          const month = new Date(payment.paymentDate).toLocaleDateString(
            "en-US",
            {
              year: "numeric",
              month: "short",
            }
          );
          revenueByMonth[month] = (revenueByMonth[month] || 0) + payment.amount;
        });
      const revenueTimeline = Object.entries(revenueByMonth).map(
        ([month, revenue]) => ({
          month,
          revenue: Math.round(revenue),
        })
      );

      setStats({
        overview: {
          totalPolicies: policies.length,
          totalClaims: claims.length,
          totalPayments: payments.length,
          totalRevenue: paymentStatsRes.data.amounts.netRevenue,
          activePolicies: policies.filter((p) => p.status === "Active").length,
          pendingClaims: claims.filter(
            (c) => c.status === "Submitted" || c.status === "UnderReview"
          ).length,
          claimApprovalRate: claimStatsRes.data.amounts.approvalRate,
          paymentSuccessRate: paymentStatsRes.data.amounts.successRate,
        },
        policyByType,
        policyByStatus,
        claimsTimeline,
        revenueTimeline,
        claimStats: claimStatsRes.data,
        paymentStats: paymentStatsRes.data,
      });
    } catch (error) {
      setError(error.response?.data?.message || "Failed to load statistics");
      console.error("Error loading statistics:", error);
    } finally {
      setLoading(false);
    }
  };

  const COLORS = {
    Life: "#8b5cf6",
    Auto: "#3b82f6",
    Health: "#10b981",
    Property: "#f59e0b",
    Active: "#10b981",
    Expired: "#ef4444",
    Cancelled: "#6b7280",
    Suspended: "#f59e0b",
  };

  if (loading) {
    return (
      <Layout>
        <LoadingSpinner message="Loading admin dashboard..." />
      </Layout>
    );
  }

  if (!stats) {
    return (
      <Layout>
        <ErrorAlert message={error || "No data available"} />
      </Layout>
    );
  }

  return (
    <Layout>
      <div>
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Admin Dashboard</h1>
          <p className="text-gray-600 mt-1">Business analytics and insights</p>
        </div>

        <ErrorAlert message={error} onClose={() => setError("")} />

        {/* Key Metrics */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          {/* Total Policies */}
          <div className="card">
            <div className="flex items-center justify-between mb-3">
              <h3 className="text-sm font-medium text-gray-600">
                Total Policies
              </h3>
              <div className="p-2 bg-blue-50 rounded-lg">
                <FiFileText className="text-blue-600" />
              </div>
            </div>
            <p className="text-3xl font-bold text-gray-900">
              {stats.overview.totalPolicies}
            </p>
            <p className="text-sm text-gray-500 mt-1">
              {stats.overview.activePolicies} active
            </p>
          </div>

          {/* Total Claims */}
          <div className="card">
            <div className="flex items-center justify-between mb-3">
              <h3 className="text-sm font-medium text-gray-600">
                Total Claims
              </h3>
              <div className="p-2 bg-green-50 rounded-lg">
                <FiAlertCircle className="text-green-600" />
              </div>
            </div>
            <p className="text-3xl font-bold text-gray-900">
              {stats.overview.totalClaims}
            </p>
            <p className="text-sm text-gray-500 mt-1">
              {stats.overview.pendingClaims} pending review
            </p>
          </div>

          {/* Total Revenue */}
          <div className="card">
            <div className="flex items-center justify-between mb-3">
              <h3 className="text-sm font-medium text-gray-600">
                Total Revenue
              </h3>
              <div className="p-2 bg-purple-50 rounded-lg">
                <FiDollarSign className="text-purple-600" />
              </div>
            </div>
            <p className="text-3xl font-bold text-gray-900">
              R{stats.overview.totalRevenue.toLocaleString()}
            </p>
            <p className="text-sm text-gray-500 mt-1">Net revenue</p>
          </div>

          {/* Success Rates */}
          <div className="card">
            <div className="flex items-center justify-between mb-3">
              <h3 className="text-sm font-medium text-gray-600">
                Success Rates
              </h3>
              <div className="p-2 bg-green-50 rounded-lg">
                <FiTrendingUp className="text-green-600" />
              </div>
            </div>
            <p className="text-3xl font-bold text-gray-900">
              {Math.round(stats.overview.claimApprovalRate)}%
            </p>
            <p className="text-sm text-gray-500 mt-1">Claim approval rate</p>
          </div>
        </div>

        {/* Charts Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
          {/* Policy Distribution by Type */}
          <div className="card">
            <h3 className="text-lg font-bold text-gray-900 mb-4">
              Policies by Type
            </h3>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={stats.policyByType}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={({ name, value }) => `${name}: ${value}`}
                  outerRadius={100}
                  fill="#8884d8"
                  dataKey="value"
                >
                  {stats.policyByType.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[entry.name]} />
                  ))}
                </Pie>
                <Tooltip />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </div>

          {/* Policy Status Distribution */}
          <div className="card">
            <h3 className="text-lg font-bold text-gray-900 mb-4">
              Policies by Status
            </h3>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={stats.policyByStatus}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="value" fill="#3b82f6">
                  {stats.policyByStatus.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[entry.name]} />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>

          {/* Claims Timeline */}
          <div className="card">
            <h3 className="text-lg font-bold text-gray-900 mb-4">
              Claims Over Time
            </h3>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={stats.claimsTimeline}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Line
                  type="monotone"
                  dataKey="claims"
                  stroke="#10b981"
                  strokeWidth={2}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>

          {/* Revenue Timeline */}
          <div className="card">
            <h3 className="text-lg font-bold text-gray-900 mb-4">
              Revenue Over Time
            </h3>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={stats.revenueTimeline}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis />
                <Tooltip formatter={(value) => `R${value.toLocaleString()}`} />
                <Legend />
                <Line
                  type="monotone"
                  dataKey="revenue"
                  stroke="#8b5cf6"
                  strokeWidth={2}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Detailed Statistics */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Claims Breakdown */}
          <div className="card">
            <h3 className="text-lg font-bold text-gray-900 mb-4">
              Claims Breakdown
            </h3>
            <div className="space-y-3">
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Submitted</span>
                <span className="font-semibold text-blue-600">
                  {stats.claimStats.byStatus.submitted}
                </span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Under Review</span>
                <span className="font-semibold text-yellow-600">
                  {stats.claimStats.byStatus.underReview}
                </span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Approved</span>
                <span className="font-semibold text-green-600">
                  {stats.claimStats.byStatus.approved}
                </span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Denied</span>
                <span className="font-semibold text-red-600">
                  {stats.claimStats.byStatus.denied}
                </span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Paid</span>
                <span className="font-semibold text-purple-600">
                  {stats.claimStats.byStatus.paid}
                </span>
              </div>
              <div className="pt-3 border-t border-gray-200">
                <div className="flex justify-between items-center">
                  <span className="text-gray-900 font-medium">
                    Approval Rate
                  </span>
                  <span className="font-bold text-green-600">
                    {Math.round(stats.claimStats.amounts.approvalRate)}%
                  </span>
                </div>
              </div>
            </div>
          </div>

          {/* Payment Breakdown */}
          <div className="card">
            <h3 className="text-lg font-bold text-gray-900 mb-4">
              Payment Breakdown
            </h3>
            <div className="space-y-3">
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Completed</span>
                <span className="font-semibold text-green-600">
                  {stats.paymentStats.byStatus.completed}
                </span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Pending</span>
                <span className="font-semibold text-yellow-600">
                  {stats.paymentStats.byStatus.pending}
                </span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Failed</span>
                <span className="font-semibold text-red-600">
                  {stats.paymentStats.byStatus.failed}
                </span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Refunded</span>
                <span className="font-semibold text-gray-600">
                  {stats.paymentStats.byStatus.refunded}
                </span>
              </div>
              <div className="pt-3 border-t border-gray-200 space-y-2">
                <div className="flex justify-between items-center">
                  <span className="text-gray-600">Total Received</span>
                  <span className="font-semibold text-green-600">
                    R{stats.paymentStats.amounts.totalReceived.toLocaleString()}
                  </span>
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-gray-600">Total Refunded</span>
                  <span className="font-semibold text-red-600">
                    R{stats.paymentStats.amounts.totalRefunded.toLocaleString()}
                  </span>
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-gray-900 font-medium">Net Revenue</span>
                  <span className="font-bold text-purple-600">
                    R{stats.paymentStats.amounts.netRevenue.toLocaleString()}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default AdminDashboard;
