export type Column<T> = {
  key: keyof T | "actions";
  title: string;
  sortable?: boolean;
  render?: (row: T) => React.ReactNode;
};

export interface DataTableProps<T extends { id?: string }> {
  title: string;
  data: T[];
  columns: Column<T>[];
  loading?: boolean;
  search: string;
  page?: number;
  pageSize?: number;
  totalPages: number;
  totalCount: number;
  pageSizeOptions?: number[];
  disableReports: boolean;
  sortField?: keyof T;
  sortDirection?: "asc" | "desc";

  onSearch: (value: string) => void;
  onSort: (field: keyof T) => void;

  onPageChange: (page: number) => void;
  onPageSizeChange: (size: number) => void;

  onExportExcel?: () => void;
  onExportPdf?: () => void;

  actions?: {
    onEdit?: (row: T) => void;
    onDelete?: (row: T) => void;
  };

  /** "view" shows an eye icon instead of pencil (e.g. error details) */
  actionMode?: "edit" | "view";
}

export interface PaginationProps {
  page: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export interface PageSizeProps {
  pageSize: number;
  pageSizeOptions: number[];
  onChange: (value: number) => void;
}

export interface PaginationRequest {
  page: number;
  pageSize: number;
  search?: string;

  sortField?: string;
  sortDirection?: "asc" | "desc";
}

export interface PagedResult<T> {
  items: T[];

  page: number;

  pageSize: number;

  totalCount: number;

  totalPages: number;
}
