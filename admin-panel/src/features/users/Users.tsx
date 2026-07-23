import { useState } from "react";
import { PlusCircle } from "lucide-react";
import { DataTable } from "../../shared/components/DataTable/DataTable";
import type { Column } from "../../shared/types/Components/DataTable";
import { useUsers } from "../../shared/hooks/useUsers";
import { formatToIranDateOnly } from "../../shared/utils/formatToIranTime";
import { CreateUserModal } from "./CreateUserModal";
import type { UserListItemDto } from "../../shared/types/UserListItemDto";

export default function Users() {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [search, setSearch] = useState("");
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [sortField, setSortField] = useState<
    keyof UserListItemDto | undefined
  >();
  const [sortDirection, setSortDirection] = useState<"asc" | "desc">("asc");

  const { data, isLoading } = useUsers({
    page,
    pageSize,
    search,
    sortField,
    sortDirection,
  });

  const users: UserListItemDto[] = data?.items ?? [];
  const totalPages: number = data?.totalPages ?? 1;
  const totalCount: number = data?.totalCount ?? 0;

  const handleSort = (field: keyof UserListItemDto) => {
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

  const columns: Column<UserListItemDto>[] = [
    { key: "fullName", title: "نام", sortable: true },
    { key: "email", title: "ایمیل", sortable: true },
    { key: "projectCount", title: "پروژه‌ها", sortable: true },
    {
      key: "createdAt",
      title: "تاریخ",
      sortable: true,
      render: (row) => formatToIranDateOnly(row.createdAt),
    },
  ];

  return (
    <>
      <div className="saas-page">
        <div className="saas-page-header">
          <div>
            <h2 className="saas-page-title">مشتریان</h2>
            <p className="saas-page-desc">مدیریت کاربران سیستم</p>
          </div>
          <button
            type="button"
            onClick={() => setIsCreateModalOpen(true)}
            className="btn-saas btn btn-sm gap-2"
          >
            <PlusCircle size={18} />
            افزودن کاربر
          </button>
        </div>

        <DataTable<UserListItemDto>
          title="لیست کاربران"
          data={users}
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
        />
      </div>

      <CreateUserModal
        open={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
      />
    </>
  );
}

