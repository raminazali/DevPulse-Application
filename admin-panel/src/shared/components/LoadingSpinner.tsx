interface LoadingSpinnerProps {
  size?: "sm" | "md" | "lg";
  className?: string;
  text?: string;
}

export function LoadingSpinner({
  size = "md",
  className = "",
  text = "در حال بارگذاری...",
}: LoadingSpinnerProps) {
  const sizeClass =
    size === "sm" ? "w-5 h-5" : size === "lg" ? "w-12 h-12" : "w-8 h-8";

  return (
    <div
      className={`flex flex-col items-center justify-center py-8 ${className}`}
    >
      <span className={`loading loading-spinner text-primary ${sizeClass}`} />
      {text && <p className="mt-3 text-sm opacity-70">{text}</p>}
    </div>
  );
}
