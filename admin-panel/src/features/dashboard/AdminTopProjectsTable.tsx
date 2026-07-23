import type { AdminTopProject } from "../../shared/types/dashboard";

interface AdminTopProjectsTableProps {
  projects: AdminTopProject[];
}

export default function AdminTopProjectsTable({
  projects,
}: AdminTopProjectsTableProps) {
  return (
    <div className="saas-card !p-0 overflow-hidden">
      <div className="border-b border-base-300 px-6 py-5">
        <h2 className="saas-card-title">برترین پروژه‌ها (تمام کاربران)</h2>
        <p className="saas-card-subtitle mt-1">پروژه‌ها با بیشترین خطا</p>
      </div>

      <div className="overflow-x-auto">
        <table className="saas-table table">
          <thead>
            <tr>
              <th className="text-start">نام پروژه</th>
              <th className="text-start">کاربر</th>
              <th className="text-end">ارورها</th>
            </tr>
          </thead>

          <tbody>
            {projects.length > 0 ? (
              projects.map((project) => (
                <tr key={project.projectId}>
                  <td className="font-medium">{project.projectName}</td>
                  <td style={{ color: "var(--muted-text)" }}>
                    {project.ownerName}
                  </td>
                  <td className="text-end">
                    <span className="badge-saas">
                      {project.errorCount.toLocaleString("fa-IR")}
                    </span>
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
                  پروژه‌ای یافت نشد.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
