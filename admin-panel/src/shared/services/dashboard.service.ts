import type { DashboardReport } from "../types/dashboard";
import { api } from "../utils/api";

export const dashboardService = {
  async getDashboard() {
    const res = await api.get<DashboardReport>("/Dashboard");
    return res.data;
  },
};
