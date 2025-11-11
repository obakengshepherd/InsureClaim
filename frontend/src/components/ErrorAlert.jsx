import { FiAlertCircle, FiX } from "react-icons/fi";

const ErrorAlert = ({ message, onClose }) => {
  if (!message) return null;

  return (
    <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg flex items-start justify-between gap-3">
      <div className="flex items-start gap-2">
        <FiAlertCircle className="text-red-500 mt-0.5 flex-shrink-0" />
        <p className="text-sm text-red-700">{message}</p>
      </div>
      {onClose && (
        <button
          onClick={onClose}
          className="text-red-500 hover:text-red-700 flex-shrink-0"
        >
          <FiX />
        </button>
      )}
    </div>
  );
};

export default ErrorAlert;
