import type { UseFormRegister, FieldError, Path } from "react-hook-form";

interface Option {
  value: string | number;
  label: string;
}

interface FormSelectProps<T extends Record<string, unknown>> {
  label: string;
  name: Path<T>;
  options: Option[];
  register: UseFormRegister<T>;
  error?: FieldError;
  placeholder?: string;
  className?: string;
}

export function FormSelect<T extends Record<string, unknown>>({
  label,
  name,
  options,
  register,
  error,
  placeholder = "انتخاب کنید",
  className = "",
}: FormSelectProps<T>) {
  return (
    <div className="space-y-1.5">
      <label className="block text-sm font-medium text-base-content">
        {label}
      </label>
      <select
        className={`select-saas select w-full ${error ? "border-error" : ""} ${className}`}
        {...register(name)}
      >
        <option value="">{placeholder}</option>
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
      {error && <p className="text-sm text-error">{error.message}</p>}
    </div>
  );
}
