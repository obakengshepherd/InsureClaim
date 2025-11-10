import axios from "axios";

// Create axios instance with base URL
const api = axios.create({
  baseURL: "http://localhost:5278/api", // Change port if needed
  headers: {
    "Content-Type": "application/json",
  },
});

// Request interceptor - Add token to every request
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor - Handle errors globally
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Unauthorized - clear token and redirect to login
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

// Auth API calls
export const authAPI = {
  register: (data) => api.post("/Auth/register", data),
  login: (data) => api.post("/Auth/login", data),
  getCurrentUser: () => api.get("/Auth/me"),
};

// Policy API calls
export const policyAPI = {
  getAll: () => api.get("/Policy"),
  getById: (id) => api.get(`/Policy/${id}`),
  create: (data) => api.post("/Policy", data),
  update: (id, data) => api.put(`/Policy/${id}`, data),
  delete: (id) => api.delete(`/Policy/${id}`),
  getUserPolicies: (userId) => api.get(`/Policy/user/${userId}`),
};

// Claim API calls
export const claimAPI = {
  getAll: () => api.get("/Claim"),
  getById: (id) => api.get(`/Claim/${id}`),
  submit: (data) => api.post("/Claim", data),
  update: (id, data) => api.put(`/Claim/${id}`, data),
  getPolicyClaims: (policyId) => api.get(`/Claim/policy/${policyId}`),
  getUserClaims: (userId) => api.get(`/Claim/user/${userId}`),
  getStatistics: () => api.get("/Claim/statistics"),
};

// Payment API calls
export const paymentAPI = {
  getAll: () => api.get("/Payment"),
  getById: (id) => api.get(`/Payment/${id}`),
  record: (data) => api.post("/Payment", data),
  update: (id, data) => api.put(`/Payment/${id}`, data),
  getPolicyPayments: (policyId) => api.get(`/Payment/policy/${policyId}`),
  getStatistics: () => api.get("/Payment/statistics"),
};

export default api;
