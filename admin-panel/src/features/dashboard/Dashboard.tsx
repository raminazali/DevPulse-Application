import { useDashboard } from "../../shared/hooks/useDashboard";
import AdminSummary from "./AdminSummary";
import AdminTopProjectsTable from "./AdminTopProjectsTable";
import HourlyErrorsChart from "./HourlyErrorsChart";
import ProjectErrorsChart from "./ProjectErrorsChart";
import RecentErrorsTable from "./RecentErrorsTable";
import SummaryStats from "./SummaryStats";
import TopExceptionTypesChart from "./TopExceptionTypesChart";
import TopUsersCard from "./TopUsersCard";

export default function DashboardPage() {
  const { data: report, isLoading, isError, error } = useDashboard();

  if (isLoading) {
    return (
      <div className="flex h-96 items-center justify-center">
        <span className="loading loading-spinner loading-lg text-primary" />
      </div>
    );
  }

  if (isError || !report) {
    return (
      <div className="saas-card">
        <div className="alert border border-error/20 bg-error/10 text-error">
          <span>
            {error instanceof Error
              ? error.message
              : "بارگذاری داشبورد با خطا مواجه شد."}
          </span>
        </div>
      </div>
    );
  }

  return (
    <div className="saas-page" dir="rtl">
      <div className="saas-page-header">
        <div>
          <h2 className="saas-page-title">نمای کلی</h2>
          <p className="saas-page-desc">
            خلاصه وضعیت خطاها، پروژه‌ها و کاربران
          </p>
        </div>
      </div>

      <SummaryStats summary={report.summary} comparison={report.comparison} />

      {report.adminSummary && <AdminSummary summary={report.adminSummary} />}

      <HourlyErrorsChart data={report.hourlyErrors} />

      <ProjectErrorsChart data={report.projectErrorDistribution} />

      <div className="grid grid-cols-1 gap-6 xl:grid-cols-3">
        <TopExceptionTypesChart data={report.topExceptionTypes} />
        <TopUsersCard users={report.topUsers} />
      </div>

      {report.adminTopProjects && (
        <AdminTopProjectsTable projects={report.adminTopProjects} />
      )}

      <RecentErrorsTable errors={report.recentErrors} />
    </div>
  );
}
