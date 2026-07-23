import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../../shared/hooks/useAuth";

export function ProtectedRoute() {
  const { user, loading } = useAuth();
  const token = localStorage.getItem("artemis-token");

  if (loading) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <span className="loading loading-spinner loading-lg text-primary" />
      </div>
    );
  }

  if (!user || !token) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}
