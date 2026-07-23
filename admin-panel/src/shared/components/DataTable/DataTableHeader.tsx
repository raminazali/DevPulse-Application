import { ArrowDownAZ, ArrowUpAZ } from "lucide-react";
import type { Column } from "../../types/Components/DataTable";

interface Props<T> {
  columns: Column<T>[];
  sortField?: keyof T;
  sortDirection?: "asc" | "desc";
  onSort: (field: keyof T) => void;
}

export function DataTableHeader<T>({
  columns,
  sortField,
  sortDirection,
  onSort,
}: Props<T>) {
  return (
    <thead>
      <tr>
        {columns.map((column) => (
          <th
            key={String(column.key)}
            className={`text-center ${
              column.sortable
                ? "cursor-pointer select-none transition-colors duration-200 hover:text-primary"
                : ""
            }`}
            onClick={() => column.sortable && onSort(column.key as keyof T)}
          >
            <div className="flex items-center justify-center gap-1.5">
              <span>{column.title}</span>
              {sortField === column.key &&
                (sortDirection === "asc" ? (
                  <ArrowUpAZ size={14} className="text-primary" />
                ) : (
                  <ArrowDownAZ size={14} className="text-primary" />
                ))}
            </div>
          </th>
        ))}
      </tr>
    </thead>
  );
}
