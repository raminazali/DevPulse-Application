import { useController } from "react-hook-form";
import type { Control, Path } from "react-hook-form";

interface FormSwitchProps<T extends Record<string, unknown>> {
  label: string;
  name: Path<T>;
  control: Control<T>;
  className?: string;
}

export function FormSwitch<T extends Record<string, unknown>>({
  label,
  name,
  control,
  className = "",
}: FormSwitchProps<T>) {
  const {
    field: { value, onChange, onBlur, ref },
  } = useController({ name, control });

  return (
    <div
      className={`flex items-center justify-between rounded-xl border border-base-300 bg-base-100 px-4 py-3 ${className}`}
    >
      <label
        htmlFor={name}
        className="cursor-pointer text-sm font-medium text-base-content"
      >
        {label}
      </label>
      <input
        id={name}
        ref={ref}
        type="checkbox"
        className="toggle toggle-primary toggle-md"
        checked={!!value}
        onChange={(e) => onChange(e.target.checked)}
        onBlur={onBlur}
      />
    </div>
  );
}
