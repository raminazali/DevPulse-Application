import type { ReactNode } from "react";
import { useController } from "react-hook-form";
import type { Control, FieldError, Path } from "react-hook-form";

interface FormInputProps<T extends Record<string, unknown>> {
  label?: string;
  name: Path<T>;
  type?: "text" | "email" | "password" | "number";
  placeholder?: string;
  control: Control<T>;
  error?: FieldError;
  className?: string;
  autoFocus?: boolean;
  startIcon?: ReactNode;
  endAdornment?: ReactNode;
}

export function FormInput<T extends Record<string, unknown>>({
  label,
  name,
  type = "text",
  placeholder,
  control,
  error,
  className = "",
  autoFocus,
  startIcon,
  endAdornment,
}: FormInputProps<T>) {
  const {
    field: { value, onChange, onBlur, ref },
  } = useController({ name, control });

  return (
    <div className="space-y-1.5">
      {label && (
        <label className="block text-sm font-medium text-base-content">
          {label}
        </label>
      )}

      <div className="relative">
        {startIcon && (
          <div className="pointer-events-none absolute inset-y-0 left-3 flex items-center text-base-content/40">
            {startIcon}
          </div>
        )}

        <input
          ref={ref}
          type={type}
          placeholder={placeholder}
          autoFocus={autoFocus}
          value={(value as string) ?? ""}
          onChange={(e) => onChange(e.target.value)}
          onBlur={onBlur}
          className={`input-saas input w-full ${startIcon ? "pl-10" : ""} ${
            endAdornment ? "pr-10" : ""
          } ${error ? "border-error focus:outline-error/40" : ""} ${className}`}
        />

        {endAdornment && (
          <div className="absolute inset-y-0 right-3 flex items-center">
            {endAdornment}
          </div>
        )}
      </div>

      {error && <p className="text-sm text-error">{error.message}</p>}
    </div>
  );
}
