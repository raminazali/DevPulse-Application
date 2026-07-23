import { AlertTriangle, FolderOpen, Users, UserCheck } from "lucide-react";
import type { AdminSummary } from "../../shared/types/dashboard";

interface AdminSummaryProps {
  summary: AdminSummary;
}

export default function AdminSummary({ summary }: AdminSummaryProps) {
  const cards = [
    {
      title: "تمام کاربران",
      value: summary.totalUsers,
      desc: `${summary.activeUsers.toLocaleString("fa-IR")} فعال`,
      icon: Users,
    },
    {
      title: "تمام پروژه‌ها",
      value: summary.totalProjects,
      desc: `${summary.activeProjects.toLocaleString("fa-IR")} فعال`,
      icon: FolderOpen,
    },
    {
      title: "تمام ارورها",
      value: summary.totalErrors,
      desc: "کل سیستم",
      icon: AlertTriangle,
    },
    {
      title: "ارورهای امروز",
      value: summary.todayErrors,
      desc: "در ۲۴ ساعت اخیر",
      icon: UserCheck,
    },
  ];

  return (
    <div>
      <div className="mb-3 flex items-center justify-between">
        <h3 className="text-sm font-semibold tracking-tight">خلاصه ادمین</h3>
        <span className="badge-saas">Admin</span>
      </div>
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-4">
        {cards.map((card) => {
          const Icon = card.icon;
          return (
            <div key={card.title} className="saas-card">
              <div className="flex items-start justify-between gap-3">
                <div>
                  <p className="saas-card-subtitle">{card.title}</p>
                  <p className="mt-2 text-2xl font-semibold tracking-tight">
                    {card.value.toLocaleString("fa-IR")}
                  </p>
                  <p
                    className="mt-2 text-xs"
                    style={{ color: "var(--muted-text)" }}
                  >
                    {card.desc}
                  </p>
                </div>
                <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-base-200 text-primary">
                  <Icon size={18} strokeWidth={1.75} />
                </div>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
