import { Activity, CalendarDays, Clock3, FolderKanban } from "lucide-react";
import type {
  DashboardSummary,
  ErrorComparison,
} from "../../shared/types/dashboard";
import { formatPercentage } from "../../shared/utils/dashboard";
import { formatRelativeWithDate } from "../../shared/utils/formatToIranTime";

interface SummaryStatsProps {
  summary: DashboardSummary;
  comparison: ErrorComparison;
}

export default function SummaryStats({
  summary,
  comparison,
}: SummaryStatsProps) {
  const cards = [
    {
      title: "تعداد ارورها",
      value: summary.totalErrors,
      desc: `${formatPercentage(comparison.monthPercentage)} این ماه`,
      icon: Activity,
    },
    {
      title: "ارورهای امروز",
      value: summary.todayErrors,
      desc: `${formatPercentage(comparison.todayPercentage)} نسبت به دیروز`,
      icon: CalendarDays,
    },
    {
      title: "ارورهای هفته",
      value: summary.weekErrors,
      desc: `${formatPercentage(comparison.weekPercentage)} نسبت به هفته قبل`,
      icon: Clock3,
    },
    {
      title: "پروژه‌های فعال",
      value: summary.activeProjects,
      desc: summary.lastErrorAt
        ? `آخرین خطا: ${formatRelativeWithDate(summary.lastErrorAt)}`
        : "اروری وجود ندارد",
      icon: FolderKanban,
    },
  ];

  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-4">
      {cards.map((card) => {
        const Icon = card.icon;
        return (
          <div
            key={card.title}
            className="saas-card group transition-all duration-200 hover:scale-[1.01]"
          >
            <div className="flex items-start justify-between gap-3">
              <div>
                <p className="saas-card-subtitle">{card.title}</p>
                <p className="mt-2 text-3xl font-semibold tracking-tight text-base-content">
                  {card.value.toLocaleString("fa-IR")}
                </p>
                <p
                  className="mt-2 text-xs"
                  style={{ color: "var(--muted-text)" }}
                >
                  {card.desc}
                </p>
              </div>
              <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-primary/15 text-primary transition-colors duration-200 group-hover:bg-primary/25">
                <Icon size={20} strokeWidth={1.75} />
              </div>
            </div>
          </div>
        );
      })}
    </div>
  );
}
