import {
  Area,
  AreaChart,
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

interface HourlyErrorsChartProps {
  data: KeyValue[];
}

export default function HourlyErrorsChart({ data }: HourlyErrorsChartProps) {
  return (
    <div className="saas-card">
      <div className="mb-4">
        <h2 className="saas-card-title">ارورهای ساعتی (امروز)</h2>
        <p className="saas-card-subtitle mt-1">روند خطا در ۲۴ ساعت اخیر</p>
      </div>

      <div className="h-80 w-full">
        <ResponsiveContainer width="100%" height="100%">
          <AreaChart data={data}>
            <defs>
              <linearGradient id="errorsGradient" x1="0" y1="0" x2="0" y2="1">
                <stop
                  offset="0%"
                  stopColor={chartColors.primary}
                  stopOpacity={0.35}
                />
                <stop
                  offset="100%"
                  stopColor={chartColors.primary}
                  stopOpacity={0}
                />
              </linearGradient>
            </defs>

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
              interval={2}
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

            <Area
              type="monotone"
              dataKey="value"
              stroke={chartColors.primary}
              fill="url(#errorsGradient)"
              strokeWidth={2.5}
            />
          </AreaChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}
