import axios from "axios";
import { toast } from "react-toastify";
import { toastConfig } from "./toast.config";

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  timeout: 10000,
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("artemis-token");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

api.interceptors.response.use(
  (response) => response,

  (error) => {
    if (!error.response) {
      toast.error("ارتباط با سرور برقرار نشد.", {
        ...toastConfig,
        toastId: "server-error",
      });

      return Promise.reject(error);
    }

    const { status, data } = error.response;

    const message =
      data?.detail ??
      data?.message ??
      data?.title ??
      "خطای غیرمنتظره رخ داده است.";

    switch (status) {
      case 400:
        toast.warning(message, {
          ...toastConfig,
          toastId: "bad-request",
        });

        break;

      case 401:
        toast.error(message, {
          ...toastConfig,
          toastId: "unauthorized",
        });

        if (!window.location.pathname.includes("/login")) {
          localStorage.removeItem("artemis-user");
          localStorage.removeItem("artemis-token");

          setTimeout(() => {
            window.location.href = "/login";
          }, 1500);
        }

        break;

      case 403:
        toast.error(message, {
          ...toastConfig,
          toastId: "forbidden",
        });

        break;

      case 404:
        toast.info(message, {
          ...toastConfig,
          toastId: "not-found",
        });

        break;

      case 409:
        toast.warning(message, {
          ...toastConfig,
          toastId: "conflict",
        });

        break;

      case 500:
        toast.error(message, {
          ...toastConfig,
          toastId: "server-error",
        });

        break;

      default:
        toast.error(message, {
          ...toastConfig,
          toastId: "unknown-error",
        });
    }

    return Promise.reject(error);
  },
);
