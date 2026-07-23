import type { PageSizeProps } from "../../types";

export function DataTablePageSize({
  pageSize,
  pageSizeOptions,
  onChange,
}: PageSizeProps) {
  return (
    <div className="flex items-center gap-2 text-sm">
      <span className="saas-card-subtitle">نمایش</span>
      <select
        className="
      select select-bordered select-sm
      pr-10 pl-10 appearance-none
    "
        value={pageSize}
        onChange={(e) => onChange(Number(e.target.value))}
      >
        {pageSizeOptions.map((size) => (
          <option key={size} value={size}>
            {size}
          </option>
        ))}
      </select>
      {/* <span className="saas-card-subtitle">ردیف</span> */}
    </div>
  );
}
