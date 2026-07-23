import { PackageSearch } from "lucide-react";

interface EmptyStateProps {
  title?: string;
  description?: string;
  icon?: React.ReactNode;
}

export function EmptyState({
  title = "داده‌ای یافت نشد",
  description = "در حال حاضر هیچ موردی وجود ندارد",
  icon,
}: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center justify-center py-16 text-center">
      {icon || <PackageSearch size={64} className="opacity-30 mb-4" />}
      <h3 className="text-lg font-semibold mb-1">{title}</h3>
      <p className="text-sm opacity-70 max-w-xs">{description}</p>
    </div>
  );
}
