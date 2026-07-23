import { createBrowserRouter } from "react-router-dom";

import MainLayout from "../layouts/MainLayout";
import Dashboard from "../../features/dashboard/Dashboard";
import LoginPage from "../../pages/LoginPage";
import { ProtectedRoute } from "./ProtectedRoute";
import { AdminRoute } from "./AdminRoute";
import Users from "../../features/users/Users";
import Projects from "../../features/projects/Projects";
import Errors from "../../features/Errors/Errors";
import UserProfile from "../../features/userProfile/UserProfile";

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginPage />,
  },

  {
    element: <ProtectedRoute />,
    children: [
      {
        path: "/",
        element: <MainLayout />,
        children: [
          {
            index: true,
            element: <Dashboard />,
          },
          {
            element: <AdminRoute />,
            children: [
              {
                path: "users",
                element: <Users />,
              },
            ],
          },
          {
            path: "projects",
            element: <Projects />,
          },
          {
            path: "errors",
            element: <Errors />,
          },
          {
            path: "profile",
            element: <UserProfile />,
          },
        ],
      },
    ],
  },
]);
