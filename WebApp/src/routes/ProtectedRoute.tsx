import { Navigate } from "react-router-dom";
import { useAuth } from "../hooks";
import { hasPermission, isRootUser } from "../permissions/Rules";
import type { JSX } from "react";
import type { ValidPermission } from "../permissions/tokens";

interface ProtectedRouteProps {
  children: JSX.Element;
  requiredPermission?: ValidPermission;
}

export default function ProtectedRoute({
  children,
  requiredPermission,
}: ProtectedRouteProps) {
  const { authUser } = useAuth();

  if (!authUser) {
    return <Navigate to="/login" replace />;
  }

  if (!requiredPermission) {
    return children;
  }

  if (isRootUser(authUser) || hasPermission(authUser, requiredPermission)) {
    return children;
  }

  return <Navigate to="/unauthorized" replace />;
}
