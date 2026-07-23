import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../../shared/hooks/useAuth";

export function AdminRoute() {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <div className="flex min-h-40 items-center justify-center">
        <span className="loading loading-spinner loading-md text-primary" />
      </div>
    );
  }

  if (!user?.isAdmin) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}
