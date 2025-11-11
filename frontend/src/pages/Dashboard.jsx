import { useState, useEffect } from "react";
import { useAuth } from "../context/AuthContext";
import { policyAPI, claimAPI, paymentAPI } from "../services/api";
import { useNavigate } from "react-router-dom";
import Layout from "../components/Layout";
import LoadingSpinner from "../components/LoadingSpinner";
import {
  FiFileText,
  FiAlertCircle,
  FiCreditCard,
  FiTrendingUp,
} from "react-icons/fi";

const Dashboard = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [stats, setStats] = useState({
    policies: 0,
    claims: 0,
    payments: 0,
    totalPaid: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchStats();
  }, []);

  const fetchStats = async () => {
    try {
      const [policiesRes, claimsRes, paymentsRes] = await Promise.all([
        policyAPI.getAll(),
        claimAPI.getAll(),
        paymentAPI.getAll(),
      ]);

      const totalPaid = paymentsRes.data
        .filter((p) => p.status === "Completed")
        .reduce((sum, p) => sum + p.amount, 0);

      setStats({
        policies: policiesRes.data.length,
        claims: claimsRes.data.length,
        payments: paymentsRes.data.length,
        totalPaid: totalPaid,
      });
    } catch (err) {
      console.error("Failed to fetch stats:", err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Layout>
        <LoadingSpinner message="Loading dashboard..." />
      </Layout>
    );
  }

  return (
    <Layout>
      <div>
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
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          {/* Policies Card */}
          <div
            className="card hover:shadow-lg transition-shadow cursor-pointer"
            onClick={() => navigate("/policies")}
          >
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600 mb-1">My Policies</p>
                <p className="text-3xl font-bold text-gray-900">
                  {stats.policies}
                </p>
                <p className="text-xs text-gray-500 mt-1">Active policies</p>
              </div>
              <div className="p-4 bg-blue-50 rounded-full">
                <FiFileText className="text-3xl text-blue-600" />
              </div>
            </div>
          </div>

          {/* Claims Card */}
          <div
            className="card hover:shadow-lg transition-shadow cursor-pointer"
            onClick={() => navigate("/claims")}
          >
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600 mb-1">My Claims</p>
                <p className="text-3xl font-bold text-gray-900">
                  {stats.claims}
                </p>
                <p className="text-xs text-gray-500 mt-1">Total claims</p>
              </div>
              <div className="p-4 bg-green-50 rounded-full">
                <FiAlertCircle className="text-3xl text-green-600" />
              </div>
            </div>
          </div>

          {/* Payments Card */}
          <div
            className="card hover:shadow-lg transition-shadow cursor-pointer"
            onClick={() => navigate("/payments")}
          >
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600 mb-1">Payments</p>
                <p className="text-3xl font-bold text-gray-900">
                  {stats.payments}
                </p>
                <p className="text-xs text-gray-500 mt-1">Total transactions</p>
              </div>
              <div className="p-4 bg-purple-50 rounded-full">
                <FiCreditCard className="text-3xl text-purple-600" />
              </div>
            </div>
          </div>

          {/* Total Paid Card */}
          <div className="card hover:shadow-lg transition-shadow">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600 mb-1">Total Paid</p>
                <p className="text-3xl font-bold text-gray-900">
                  R{stats.totalPaid.toLocaleString()}
                </p>
                <p className="text-xs text-gray-500 mt-1">All time</p>
              </div>
              <div className="p-4 bg-green-50 rounded-full">
                <FiTrendingUp className="text-3xl text-green-600" />
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
            <button
              onClick={() => navigate("/policies")}
              className="btn btn-primary py-4 text-left flex items-center justify-between"
            >
              <span>View Policies</span>
              <FiFileText />
            </button>
            <button
              onClick={() => navigate("/claims")}
              className="btn btn-primary py-4 text-left flex items-center justify-between"
            >
              <span>Submit Claim</span>
              <FiAlertCircle />
            </button>
            <button
              onClick={() => navigate("/payments")}
              className="btn btn-primary py-4 text-left flex items-center justify-between"
            >
              <span>Payment History</span>
              <FiCreditCard />
            </button>
          </div>
        </div>

        {/* Info Card */}
        <div className="mt-8 card bg-primary-50 border border-primary-200">
          <div className="flex items-start gap-3">
            <div className="p-2 bg-primary-600 rounded-lg flex-shrink-0">
              <FiTrendingUp className="text-white" />
            </div>
            <div>
              <h3 className="font-semibold text-gray-900 mb-1">
                Your insurance dashboard is ready!
              </h3>
              <p className="text-sm text-gray-600">
                Manage your policies, submit claims, and track payments all in
                one place.
              </p>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default Dashboard;
