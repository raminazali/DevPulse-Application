import type { TopUser } from "../../shared/types/dashboard";

interface TopUsersCardProps {
  users: TopUser[];
}

export default function TopUsersCard({ users }: TopUsersCardProps) {
  const maxErrors = Math.max(...users.map((user) => user.errorCount), 1);

  return (
    <div className="saas-card xl:col-span-2">
      <div className="mb-4">
        <h2 className="saas-card-title">بیشترین خطاها بر اساس کاربر</h2>
        <p className="saas-card-subtitle mt-1">رتبه‌بندی کاربران بر اساس خطا</p>
      </div>

      {users.length === 0 ? (
        <div
          className="flex h-48 items-center justify-center text-sm"
          style={{ color: "var(--muted-text)" }}
        >
          داده کاربر یافت نشد.
        </div>
      ) : (
        <div className="mt-2 space-y-5">
          {users.map((user, index) => (
            <div key={user.userId}>
              <div className="mb-2 flex items-center justify-between gap-3">
                <div className="flex min-w-0 items-center gap-3">
                  <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/15 text-xs font-semibold text-primary">
                    {index + 1}
                  </span>
                  <div className="min-w-0">
                    <p className="truncate font-medium">{user.fullName}</p>
                    <p
                      className="text-xs"
                      style={{ color: "var(--muted-text)" }}
                    >
                      {user.projectCount.toLocaleString("fa-IR")} پروژه
                    </p>
                  </div>
                </div>
                <span className="shrink-0 text-sm font-semibold tabular-nums">
                  {user.errorCount.toLocaleString("fa-IR")}
                </span>
              </div>

              <progress
                className="progress progress-primary h-2 w-full"
                value={user.errorCount}
                max={maxErrors}
              />
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
