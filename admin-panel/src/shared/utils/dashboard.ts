import type { CSSProperties } from "react";

export function formatPercentage(value: number) {
  const rounded = Math.round(value * 10) / 10;
  return `${rounded > 0 ? "↗" : rounded < 0 ? "↓" : "→"} ${Math.abs(rounded)}%`;
}

export function formatTime(iso: string) {
  return new Date(iso).toLocaleString(undefined, {
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

// Green-only chart palette (theme-aware via CSS variables)
export const chartColors = {
  primary: "var(--color-primary)",
  primarySoft: "color-mix(in oklab, var(--color-primary) 45%, transparent)",
  gray: "var(--muted-text)",
  base100: "var(--color-base-100)",
  base300: "var(--color-base-300)",
  content: "var(--color-base-content)",
  border: "var(--card-border)",
  error: "var(--color-error)",
};

export const tooltipStyle: CSSProperties = {
  backgroundColor: chartColors.base100,
  border: `1px solid ${chartColors.border}`,
  borderRadius: "12px",
  color: chartColors.content,
  boxShadow: "0 8px 24px rgb(0 0 0 / 0.08)",
  padding: "10px 12px",
};

export const tooltipLabelStyle: CSSProperties = {
  color: chartColors.content,
  fontWeight: 600,
};

/** Monochrome green scale for pie slices — no rainbow */
export const pieSliceColors = [
  "#4ade80",
  "#22c55e",
  "#16a34a",
  "#86efac",
  "#a3a3a3",
  "#737373",
];
