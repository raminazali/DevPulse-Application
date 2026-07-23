import type { RecentError } from "../../shared/types/dashboard";
import { formatToIranDateTime } from "../../shared/utils/formatToIranTime";

interface RecentErrorsTableProps {
  errors: RecentError[];
}

export default function RecentErrorsTable({ errors }: RecentErrorsTableProps) {
  return (
    <div className="saas-card !p-0 overflow-hidden">
      <div className="border-b border-base-300 px-6 py-5">
        <h2 className="saas-card-title">آخرین خطاها</h2>
        <p className="saas-card-subtitle mt-1">رویدادهای اخیر سیستم</p>
      </div>

      <div className="overflow-x-auto">
        <table className="saas-table table">
          <thead>
            <tr>
              <th className="text-start">پروژه</th>
              <th className="text-start">نوع خطا</th>
              <th className="text-start">زمان رخداد</th>
            </tr>
          </thead>

          <tbody>
            {errors.length > 0 ? (
              errors.map((error) => (
                <tr key={error.errorId}>
                  <td className="font-medium">{error.projectName}</td>
                  <td>
                    <span className="badge-saas-muted font-mono text-[11px]">
                      {error.exceptionType}
                    </span>
                  </td>
                  <td style={{ color: "var(--muted-text)" }}>
                    {formatToIranDateTime(error.createdAt)}
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td
                  colSpan={3}
                  className="py-10 text-center text-sm"
                  style={{ color: "var(--muted-text)" }}
                >
                  رخداد خطایی یافت نشد.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
