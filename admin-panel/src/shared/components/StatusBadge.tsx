interface StatusBadgeProps {
  status: boolean | string;
  trueText?: string;
  falseText?: string;
  className?: string;
}

export function StatusBadge({
  status,
  trueText = "فعال",
  falseText = "غیرفعال",
  className = "",
}: StatusBadgeProps) {
  const isActive = typeof status === "boolean" ? status : status === "true";

  return (
    <span
      className={`${isActive ? "badge-saas" : "badge-saas-danger"} ${className}`}
    >
      {isActive ? trueText : falseText}
    </span>
  );
}
