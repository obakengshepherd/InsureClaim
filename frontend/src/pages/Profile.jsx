import { useState, useEffect } from "react";
import { useAuth } from "../context/AuthContext";
import { authAPI } from "../services/api";
import Layout from "../components/Layout";
import LoadingSpinner from "../components/LoadingSpinner";
import ErrorAlert from "../components/ErrorAlert";
import toast from "react-hot-toast";
import {
  FiUser,
  FiMail,
  FiPhone,
  FiShield,
  FiCalendar,
  FiEdit2,
  FiSave,
  FiX,
} from "react-icons/fi";

const Profile = () => {
  const { user: _user, logout } = useAuth();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [isEditing, setIsEditing] = useState(false);
  const [profileData, setProfileData] = useState(null);
  const [formData, setFormData] = useState({
    fullName: "",
    phoneNumber: "",
  });

  useEffect(() => {
    fetchProfile();
  }, []);

  const fetchProfile = async () => {
    try {
      setLoading(true);
      const response = await authAPI.getCurrentUser();
      setProfileData(response.data);
      setFormData({
        fullName: response.data.fullName,
        phoneNumber: response.data.phoneNumber,
      });
    } catch (error) {
      setError(error.response?.data?.message || "Failed to load profile");
      console.error("Error loading profile:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = () => {
    setIsEditing(true);
  };

  const handleCancel = () => {
    setIsEditing(false);
    setFormData({
      fullName: profileData.fullName,
      phoneNumber: profileData.phoneNumber,
    });
  };

  const handleSave = async () => {
    // Note: This would require a backend endpoint to update user profile
    // For now, we'll just show a toast
    setSaving(true);

    setTimeout(() => {
      toast.success("Profile update feature coming soon!");
      setIsEditing(false);
      setSaving(false);
    }, 1000);
  };

  if (loading) {
    return (
      <Layout>
        <LoadingSpinner message="Loading profile..." />
      </Layout>
    );
  }

  if (!profileData) {
    return (
      <Layout>
        <ErrorAlert message={error || "Profile not found"} />
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-4xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">My Profile</h1>
          <p className="text-gray-600 mt-1">Manage your account information</p>
        </div>

        <ErrorAlert message={error} onClose={() => setError("")} />

        {/* Profile Card */}
        <div className="card">
          {/* Header Section */}
          <div className="flex items-center justify-between mb-6 pb-6 border-b border-gray-200">
            <div className="flex items-center gap-4">
              <div className="w-20 h-20 bg-primary-100 rounded-full flex items-center justify-center">
                <FiUser className="text-4xl text-primary-600" />
              </div>
              <div>
                <h2 className="text-2xl font-bold text-gray-900">
                  {profileData.fullName}
                </h2>
                <p className="text-gray-600">{profileData.email}</p>
              </div>
            </div>
            {!isEditing ? (
              <button
                onClick={handleEdit}
                className="btn btn-primary flex items-center gap-2"
              >
                <FiEdit2 />
                Edit Profile
              </button>
            ) : (
              <div className="flex gap-2">
                <button
                  onClick={handleCancel}
                  className="btn btn-secondary flex items-center gap-2"
                  disabled={saving}
                >
                  <FiX />
                  Cancel
                </button>
                <button
                  onClick={handleSave}
                  className="btn btn-primary flex items-center gap-2"
                  disabled={saving}
                >
                  <FiSave />
                  {saving ? "Saving..." : "Save"}
                </button>
              </div>
            )}
          </div>

          {/* Profile Information */}
          <div className="space-y-6">
            {/* Full Name */}
            <div>
              <label className="flex items-center gap-2 text-sm font-medium text-gray-700 mb-2">
                <FiUser className="text-gray-400" />
                Full Name
              </label>
              {isEditing ? (
                <input
                  type="text"
                  className="input"
                  value={formData.fullName}
                  onChange={(e) =>
                    setFormData({ ...formData, fullName: e.target.value })
                  }
                />
              ) : (
                <p className="text-gray-900 text-lg">{profileData.fullName}</p>
              )}
            </div>

            {/* Email (Read-only) */}
            <div>
              <label className="flex items-center gap-2 text-sm font-medium text-gray-700 mb-2">
                <FiMail className="text-gray-400" />
                Email Address
              </label>
              <p className="text-gray-900 text-lg">{profileData.email}</p>
              <p className="text-xs text-gray-500 mt-1">
                Email cannot be changed
              </p>
            </div>

            {/* Phone Number */}
            <div>
              <label className="flex items-center gap-2 text-sm font-medium text-gray-700 mb-2">
                <FiPhone className="text-gray-400" />
                Phone Number
              </label>
              {isEditing ? (
                <input
                  type="tel"
                  className="input"
                  value={formData.phoneNumber}
                  onChange={(e) =>
                    setFormData({ ...formData, phoneNumber: e.target.value })
                  }
                />
              ) : (
                <p className="text-gray-900 text-lg">
                  {profileData.phoneNumber}
                </p>
              )}
            </div>

            {/* Role (Read-only) */}
            <div>
              <label className="flex items-center gap-2 text-sm font-medium text-gray-700 mb-2">
                <FiShield className="text-gray-400" />
                Account Type
              </label>
              <span className="inline-block px-4 py-2 bg-primary-100 text-primary-700 rounded-full font-medium">
                {profileData.role}
              </span>
            </div>

            {/* Account Status */}
            <div>
              <label className="flex items-center gap-2 text-sm font-medium text-gray-700 mb-2">
                <FiCalendar className="text-gray-400" />
                Account Status
              </label>
              <div className="flex items-center gap-3">
                <span
                  className={`inline-block px-3 py-1 rounded-full text-sm font-medium ${
                    profileData.isActive
                      ? "bg-green-100 text-green-700"
                      : "bg-red-100 text-red-700"
                  }`}
                >
                  {profileData.isActive ? "Active" : "Inactive"}
                </span>
                <span className="text-gray-600 text-sm">
                  Member since{" "}
                  {new Date(profileData.createdAt).toLocaleDateString()}
                </span>
              </div>
            </div>
          </div>
        </div>

        {/* Actions Card */}
        <div className="card mt-6">
          <h3 className="text-lg font-bold text-gray-900 mb-4">
            Account Actions
          </h3>
          <div className="space-y-3">
            <button
              onClick={() => toast.info("Password change feature coming soon!")}
              className="w-full text-left px-4 py-3 border border-gray-200 rounded-lg hover:bg-gray-50 transition"
            >
              <p className="font-medium text-gray-900">Change Password</p>
              <p className="text-sm text-gray-600">
                Update your account password
              </p>
            </button>
            <button
              onClick={() => {
                if (confirm("Are you sure you want to logout?")) {
                  logout();
                  toast.success("Logged out successfully");
                }
              }}
              className="w-full text-left px-4 py-3 border border-red-200 rounded-lg hover:bg-red-50 transition text-red-700"
            >
              <p className="font-medium">Logout</p>
              <p className="text-sm">Sign out of your account</p>
            </button>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default Profile;
