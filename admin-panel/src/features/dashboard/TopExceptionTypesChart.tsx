import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from "recharts";

import type { KeyValue } from "../../shared/types/dashboard";
import {
  chartColors,
  pieSliceColors,
  tooltipLabelStyle,
  tooltipStyle,
} from "../../shared/utils/dashboard";

interface TopExceptionTypesChartProps {
  data: KeyValue[];
}

export default function TopExceptionTypesChart({
  data,
}: TopExceptionTypesChartProps) {
  return (
    <div className="saas-card">
      <div className="mb-4">
        <h2 className="saas-card-title">بیشترین نوع خطاها</h2>
        <p className="saas-card-subtitle mt-1">توزیع exception types</p>
      </div>

      <div className="h-72">
        <ResponsiveContainer width="100%" height="100%">
          <PieChart>
            <Pie
              data={data}
              dataKey="value"
              nameKey="label"
              cx="50%"
              cy="50%"
              innerRadius={55}
              outerRadius={90}
              paddingAngle={3}
              stroke="none"
            >
              {data.map((_, index) => (
                <Cell
                  key={index}
                  fill={pieSliceColors[index % pieSliceColors.length]}
                />
              ))}
            </Pie>

            <Tooltip
              contentStyle={tooltipStyle}
              labelStyle={tooltipLabelStyle}
              itemStyle={{ color: chartColors.content }}
            />
          </PieChart>
        </ResponsiveContainer>
      </div>

      {data.length === 0 && (
        <div className="py-6 text-center text-sm" style={{ color: "var(--muted-text)" }}>
          داده‌ای برای نمایش وجود ندارد.
        </div>
      )}

      {data.length > 0 && (
        <ul className="mt-2 space-y-2">
          {data.slice(0, 5).map((item, index) => (
            <li
              key={item.label}
              className="flex items-center justify-between text-sm"
            >
              <span className="flex items-center gap-2">
                <span
                  className="h-2.5 w-2.5 rounded-full"
                  style={{
                    backgroundColor:
                      pieSliceColors[index % pieSliceColors.length],
                  }}
                />
                <span className="truncate max-w-[10rem]">{item.label}</span>
              </span>
              <span className="font-medium tabular-nums">{item.value}</span>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
