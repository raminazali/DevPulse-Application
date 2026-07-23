import type { UseFormRegister, FieldError, Path } from "react-hook-form";

interface FormTextareaProps<T extends Record<string, unknown>> {
  label: string;
  name?: Path<T>;
  value?: string;
  onChange?: (value: string) => void;
  placeholder?: string;
  rows?: number;
  register?: UseFormRegister<T>;
  error?: FieldError;
  className?: string;
  readOnly?: boolean;
}

export function FormTextarea<T extends Record<string, unknown>>({
  label,
  name,
  value,
  onChange,
  placeholder,
  rows = 4,
  register,
  error,
  className = "",
  readOnly = false,
}: FormTextareaProps<T>) {
  const textareaProps =
    register && name
      ? register(name)
      : {
          value: value ?? "",
          onChange: (e: React.ChangeEvent<HTMLTextAreaElement>) =>
            onChange?.(e.target.value),
        };

  return (
    <div className="space-y-1.5">
      <label className="block text-sm font-medium text-base-content">
        {label}
      </label>

      <textarea
        {...textareaProps}
        className={`textarea-saas textarea w-full ${
          readOnly ? "bg-base-200" : ""
        } ${error ? "border-error" : ""} ${className}`}
        placeholder={placeholder}
        rows={rows}
        readOnly={readOnly}
      />

      {error && <p className="text-sm text-error">{error.message}</p>}
    </div>
  );
}
