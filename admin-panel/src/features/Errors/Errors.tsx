import { useState } from "react";
import type { ErrorGroup } from "../../shared/types/errorsType";
import { useErrors } from "../../shared/hooks/useErrors";
import type { Column } from "../../shared/types";
import { formatRelativeWithDate } from "../../shared/utils/formatToIranTime";
import { DataTable } from "../../shared/components/DataTable/DataTable";
import ShowErrorDetailModal from "./ShowErrorDetailModal";

export default function Errors() {
  const [showItem, setShowItem] = useState<ErrorGroup | null>(null);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [search, setSearch] = useState("");
  const [sortField, setSortField] = useState<keyof ErrorGroup | undefined>();
  const [sortDirection, setSortDirection] = useState<"asc" | "desc">("asc");

  const { data, isLoading } = useErrors({
    page,
    pageSize,
    search,
    sortField,
    sortDirection,
  });

  const errors: ErrorGroup[] = data?.items ?? [];
  const totalPages: number = data?.totalPages ?? 1;
  const totalCount: number = data?.totalCount ?? 0;

  const handleSort = (field: keyof ErrorGroup) => {
    if (sortField === field) {
      setSortDirection((prev) => (prev === "asc" ? "desc" : "asc"));
    } else {
      setSortField(field);
      setSortDirection("asc");
    }
    setPage(1);
  };

  const handleSearch = (value: string) => {
    setSearch(value);
    setPage(1);
  };

  const columns: Column<ErrorGroup>[] = [
    {
      key: "projectName",
      title: "نام پروژه",
      sortable: true,
      render: (row) => (
        <span className="badge-saas">{row.projectName}</span>
      ),
    },
    {
      key: "exceptionType",
      title: "نوع خطا",
      sortable: true,
      render: (row) => (
        <span className="badge-saas-muted font-mono text-[11px]">
          {row.exceptionType}
        </span>
      ),
    },
    {
      key: "method",
      title: "متد",
      sortable: true,
      render: (row) => {
        const method = row.method?.toUpperCase() || "—";
        const isDelete = method === "DELETE";
        return (
          <span
            className={`${
              isDelete ? "badge-saas-danger" : "badge-saas"
            } font-semibold uppercase tracking-wider`}
          >
            {method}
          </span>
        );
      },
    },
    {
      key: "createdAt",
      title: "تاریخ ایجاد",
      sortable: true,
      render: (row) => (
        <div className="text-sm" style={{ color: "var(--muted-text)" }}>
          {formatRelativeWithDate(row.createdAt)}
        </div>
      ),
    },
    {
      key: "actions",
      title: "عملیات",
    },
  ];

  return (
    <>
      <div className="saas-page">
        <div className="saas-page-header">
          <div>
            <h2 className="saas-page-title">مدیریت خطا</h2>
            <p className="saas-page-desc">پیگیری و بررسی خطاهای ثبت‌شده</p>
          </div>
        </div>

        <DataTable<ErrorGroup>
          title="لیست ارورها"
          data={errors}
          columns={columns}
          loading={isLoading}
          search={search}
          page={page}
          pageSize={pageSize}
          totalPages={totalPages}
          totalCount={totalCount}
          sortField={sortField}
          sortDirection={sortDirection}
          onSearch={handleSearch}
          onPageChange={setPage}
          onPageSizeChange={(size) => {
            setPageSize(size);
            setPage(1);
          }}
          onSort={handleSort}
          disableReports={true}
          actionMode="view"
          actions={{
            onEdit: (row) => setShowItem(row),
          }}
        />
      </div>

      <ShowErrorDetailModal
        open={!!showItem}
        errorId={showItem?.id}
        onClose={() => setShowItem(null)}
      />
    </>
  );
}
