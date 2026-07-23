import React from "react";
import ReactDOM from "react-dom/client";
import { RouterProvider } from "react-router-dom";
import { QueryClientProvider } from "@tanstack/react-query";
import { ToastContainer } from "react-toastify";
import { router } from "./app/router";
import { AuthProvider } from "./shared/context/AuthContext";
import { queryClient } from "./App";
import { toastConfig } from "./shared/utils/toast.config";
import "react-toastify/dist/ReactToastify.css";
import "./index.css";
import "./App.css";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <RouterProvider router={router} />
        <ToastContainer {...toastConfig} />
      </AuthProvider>
    </QueryClientProvider>
  </React.StrictMode>,
);
