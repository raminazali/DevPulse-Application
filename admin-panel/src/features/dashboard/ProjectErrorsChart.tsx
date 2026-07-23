import {
  Bar,
  BarChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import type { KeyValue } from "../../shared/types/dashboard";
import {
  chartColors,
  tooltipLabelStyle,
  tooltipStyle,
} from "../../shared/utils/dashboard";

interface ProjectErrorsChartProps {
  data: KeyValue[];
}

export default function ProjectErrorsChart({ data }: ProjectErrorsChartProps) {
  return (
    <div className="saas-card">
      <div className="mb-4">
        <h2 className="saas-card-title">ارورها بر اساس پروژه</h2>
        <p className="saas-card-subtitle mt-1">توزیع خطا بین پروژه‌ها</p>
      </div>

      <div className="h-72 w-full">
        <ResponsiveContainer width="100%" height="100%">
          <BarChart data={data}>
            <CartesianGrid
              stroke={chartColors.border}
              strokeDasharray="3 3"
              vertical={false}
            />

            <XAxis
              dataKey="label"
              tick={{ fill: chartColors.gray, fontSize: 12 }}
              axisLine={false}
              tickLine={false}
            />

            <YAxis
              allowDecimals={false}
              tick={{ fill: chartColors.gray, fontSize: 12 }}
              axisLine={false}
              tickLine={false}
            />

            <Tooltip
              contentStyle={tooltipStyle}
              labelStyle={tooltipLabelStyle}
              itemStyle={{ color: chartColors.primary }}
            />

            <Bar
              dataKey="value"
              fill={chartColors.primary}
              radius={[10, 10, 0, 0]}
              maxBarSize={48}
            />
          </BarChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}
