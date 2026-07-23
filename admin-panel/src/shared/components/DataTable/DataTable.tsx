import { Edit2Icon, EyeIcon, Trash2Icon } from "lucide-react";
import type { DataTableProps } from "../../types/Components/DataTable";
import { DataTableHeader } from "./DataTableHeader";
import { DataTablePageSize } from "./DataTablePageSize";
import { DataTablePagination } from "./DataTablePagination";

export function DataTable<T extends { id?: string }>({
  title,
  data,
  columns,
  loading = false,
  search,
  page = 1,
  pageSize = 10,
  totalPages,
  totalCount,
  pageSizeOptions = [5, 10, 20, 50],
  sortField,
  sortDirection,
  onSearch,
  onSort,
  onPageChange,
  onPageSizeChange,
  onExportExcel,
  onExportPdf,
  disableReports,
  actions,
  actionMode = "edit",
}: DataTableProps<T>) {
  return (
    <div className="saas-table-wrap">
      {/* Header */}
      <div className="flex flex-col gap-4 border-b border-base-300 p-5 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="saas-card-title">{title}</h2>
          <p className="saas-card-subtitle mt-1">
            تعداد کل رکوردها: {totalCount.toLocaleString("fa-IR")}
          </p>
        </div>

        <div className="flex flex-wrap items-center gap-2">
          <input
            className="input-saas input input-sm w-full max-w-xs sm:w-56"
            value={search}
            onChange={(e) => onSearch(e.target.value)}
            placeholder="جستجو..."
          />

          {!disableReports && (
            <>
              <button
                type="button"
                onClick={onExportExcel}
                className="btn-saas btn btn-sm"
              >
                Excel
              </button>
              <button
                type="button"
                onClick={onExportPdf}
                className="btn btn-sm rounded-xl border border-base-300 bg-base-100 transition-all duration-200 hover:bg-base-200"
              >
                PDF
              </button>
            </>
          )}
        </div>
      </div>

      {/* Table */}
      <div className="overflow-x-auto">
        <table className="saas-table table">
          <DataTableHeader
            columns={columns}
            sortField={sortField}
            sortDirection={sortDirection}
            onSort={onSort}
          />

          <tbody>
            {loading ? (
              <tr>
                <td colSpan={columns.length} className="py-16 text-center">
                  <span className="loading loading-spinner loading-md text-primary" />
                  <p className="saas-card-subtitle mt-3">در حال بارگذاری...</p>
                </td>
              </tr>
            ) : data.length === 0 ? (
              <tr>
                <td colSpan={columns.length} className="py-16 text-center">
                  <p className="font-medium text-base-content">
                    داده‌ای یافت نشد
                  </p>
                  <p className="saas-card-subtitle mt-1">
                    فیلتر یا جستجوی خود را تغییر دهید
                  </p>
                </td>
              </tr>
            ) : (
              data.map((row, index) => (
                <tr key={row.id ?? index} className="hover:bg-transparent">
                  {columns.map((column) => (
                    <td key={String(column.key)} className="text-center">
                      {column.key === "actions" ? (
                        <div className="flex justify-center gap-1.5">
                          {actions?.onEdit && (
                            <button
                              type="button"
                              className="btn btn-ghost btn-sm rounded-xl text-primary transition-all duration-200 hover:bg-primary/10 hover:scale-[1.02] active:scale-[0.98]"
                              onClick={() => actions.onEdit!(row)}
                              title={
                                actionMode === "view" ? "مشاهده" : "ویرایش"
                              }
                            >
                              {actionMode === "view" ? (
                                <EyeIcon size={16} />
                              ) : (
                                <Edit2Icon size={16} />
                              )}
                            </button>
                          )}

                          {actions?.onDelete && (
                            <button
                              type="button"
                              className="btn btn-ghost btn-sm rounded-xl text-error transition-all duration-200 hover:bg-error/10 hover:scale-[1.02] active:scale-[0.98]"
                              onClick={() => actions.onDelete!(row)}
                              title="حذف"
                            >
                              <Trash2Icon size={16} />
                            </button>
                          )}
                        </div>
                      ) : column.render ? (
                        column.render(row)
                      ) : (
                        String(row[column.key as keyof T] ?? "")
                      )}
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Footer */}
      <div className="flex flex-col gap-3 border-t border-base-300 p-4 sm:flex-row sm:items-center sm:justify-between">
        <DataTablePageSize
          pageSize={pageSize}
          pageSizeOptions={pageSizeOptions}
          onChange={onPageSizeChange}
        />
        <DataTablePagination
          page={page}
          totalPages={totalPages}
          onPageChange={onPageChange}
        />
      </div>
    </div>
  );
}
