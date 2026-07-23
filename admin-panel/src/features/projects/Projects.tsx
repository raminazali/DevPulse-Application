import { useState } from "react";
import { PlusCircle } from "lucide-react";
import { DataTable } from "../../shared/components/DataTable/DataTable";
import type { Column } from "../../shared/types/Components/DataTable";
import { useProjects } from "../../shared/hooks/useProjects";
import { formatToIranDateOnly } from "../../shared/utils/formatToIranTime";
import { CreateProjectModal } from "./CreateProjectModal";
import { EditProjectModal } from "./EditProjectModal";
import { DeleteProjectModal } from "./DeleteProjectModal";
import type { ProjectListItemDto } from "../../shared/types/ProjectListItemDto";

export default function Projects() {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [search, setSearch] = useState("");
  const [sortField, setSortField] = useState<
    keyof ProjectListItemDto | undefined
  >();
  const [sortDirection, setSortDirection] = useState<"asc" | "desc">("asc");
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [editProject, setEditProject] = useState<ProjectListItemDto | null>(
    null,
  );
  const [deleteProject, setDeleteProject] = useState<ProjectListItemDto | null>(
    null,
  );

  const { data, isLoading } = useProjects({
    page,
    pageSize,
    search,
    sortField,
    sortDirection,
  });

  const projects: ProjectListItemDto[] = data?.items ?? [];
  const totalPages: number = data?.totalPages ?? 1;
  const totalCount: number = data?.totalCount ?? 0;

  const handleSort = (field: keyof ProjectListItemDto) => {
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

  const columns: Column<ProjectListItemDto>[] = [
    { key: "name", title: "نام پروژه", sortable: true },
    {
      key: "isActive",
      title: "وضعیت",
      render: (row) => (
        <span
          className={row.isActive ? "badge-saas" : "badge-saas-danger"}
        >
          {row.isActive ? "فعال" : "غیرفعال"}
        </span>
      ),
    },
    {
      key: "errorCount",
      title: "تعداد خطاها",
      sortable: true,
    },
    {
      key: "createdAt",
      title: "تاریخ",
      sortable: true,
      render: (row) => formatToIranDateOnly(row.createdAt),
    },
    { key: "actions", title: "عملیات" },
  ];

  return (
    <>
      <div className="saas-page">
        <div className="saas-page-header">
          <div>
            <h2 className="saas-page-title">پروژه‌ها</h2>
            <p className="saas-page-desc">مدیریت پروژه‌ها و وضعیت آن‌ها</p>
          </div>
          <button
            type="button"
            onClick={() => setIsCreateModalOpen(true)}
            className="btn-saas btn btn-sm gap-2"
          >
            <PlusCircle size={18} />
            افزودن پروژه
          </button>
        </div>

        <DataTable<ProjectListItemDto>
          title="لیست پروژه‌ها"
          data={projects}
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
          onSort={handleSort}
          onPageChange={setPage}
          disableReports={true}
          onPageSizeChange={(size) => {
            setPageSize(size);
            setPage(1);
          }}
          actions={{
            onEdit: (row) => setEditProject(row),
            onDelete: (row) => setDeleteProject(row),
          }}
        />
      </div>

      <CreateProjectModal
        open={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
      />

      {editProject && (
        <EditProjectModal
          open={!!editProject}
          project={editProject}
          onClose={() => setEditProject(null)}
        />
      )}

      <DeleteProjectModal
        open={!!deleteProject}
        project={deleteProject}
        onClose={() => setDeleteProject(null)}
      />
    </>
  );
}
