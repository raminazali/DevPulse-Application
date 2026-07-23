import type { PaginationProps } from "../../types/Components/DataTable";

export function DataTablePagination({
  page,
  totalPages,
  onPageChange,
}: PaginationProps) {
  const getPages = (): (number | "...")[] => {
    const pages: (number | "...")[] = [];
    const delta = 1;
    const range: number[] = [];

    for (
      let i = Math.max(2, page - delta);
      i <= Math.min(totalPages - 1, page + delta);
      i++
    ) {
      range.push(i);
    }

    if (page - delta > 2) {
      pages.push(1, "...");
    } else {
      pages.push(1);
    }

    pages.push(...range);

    if (page + delta < totalPages - 1) {
      pages.push("...");
    }

    if (totalPages > 1) {
      pages.push(totalPages);
    }

    return pages;
  };

  const pages = getPages();

  return (
    <div className="join">
      <button
        type="button"
        className="btn join-item btn-sm rounded-xl border border-base-300 bg-base-100 transition-all duration-200 disabled:opacity-40"
        disabled={page === 1}
        onClick={() => onPageChange(page - 1)}
      >
        «
      </button>

      {pages.map((p, idx) =>
        p === "..." ? (
          <button
            key={`dots-${idx}`}
            type="button"
            className="btn join-item btn-sm btn-disabled border border-base-300 bg-base-100"
          >
            ...
          </button>
        ) : (
          <button
            key={p}
            type="button"
            className={`btn join-item btn-sm border transition-all duration-200 ${
              p === page
                ? "border-primary bg-primary text-primary-content shadow-sm"
                : "border-base-300 bg-base-100 hover:border-primary/40"
            }`}
            onClick={() => onPageChange(p)}
          >
            {p}
          </button>
        ),
      )}

      <button
        type="button"
        className="btn join-item btn-sm rounded-xl border border-base-300 bg-base-100 transition-all duration-200 disabled:opacity-40"
        disabled={page === totalPages || totalPages === 0}
        onClick={() => onPageChange(page + 1)}
      >
        »
      </button>
    </div>
  );
}
