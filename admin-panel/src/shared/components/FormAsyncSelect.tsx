import { useState, useRef, useEffect } from "react";
import { ChevronDown } from "lucide-react";
import type {
  UseFormRegister,
  Path,
  FieldError,
  UseFormSetValue,
  UseFormWatch,
  FieldValues,
  PathValue,
} from "react-hook-form";
import { useInfiniteUsers } from "../hooks/useUsers";

interface FormAsyncSelectProps<T extends FieldValues> {
  label: string;
  name: Path<T>;
  register: UseFormRegister<T>;
  watch: UseFormWatch<T>;
  setValue: UseFormSetValue<T>;
  error?: FieldError;
  placeholder?: string;
}

export function FormAsyncSelect<T extends FieldValues>({
  label,
  name,
  register,
  watch,
  setValue,
  error,
  placeholder = "انتخاب کنید",
}: FormAsyncSelectProps<T>) {
  const [open, setOpen] = useState(false);
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [selectedLabel, setSelectedLabel] = useState<string | null>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  const listRef = useRef<HTMLUListElement>(null);

  const selectedId = watch(name) as string | undefined;

  useEffect(() => {
    const t = setTimeout(() => setDebouncedSearch(search), 300);
    return () => clearTimeout(t);
  }, [search]);

  const { data, fetchNextPage, hasNextPage, isFetchingNextPage, isLoading } =
    useInfiniteUsers(debouncedSearch);

  const users = data?.pages.flatMap((p) => p.items) ?? [];

  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (
        containerRef.current &&
        !containerRef.current.contains(e.target as Node)
      ) {
        setOpen(false);
      }
    };
    document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, []);

  const handleScroll = () => {
    const el = listRef.current;
    if (!el || !hasNextPage || isFetchingNextPage) return;
    if (el.scrollTop + el.clientHeight >= el.scrollHeight - 40) {
      fetchNextPage();
    }
  };

  const selectedUser = users.find((u) => u.id === selectedId);

  const handleSelect = (id: string, fullName: string) => {
    setSelectedLabel(fullName);
    setValue(name, id as PathValue<T, Path<T>>, {
      shouldValidate: true,
      shouldDirty: true,
    });
    setOpen(false);
  };

  const displayLabel = selectedId
    ? (selectedLabel ?? selectedUser?.fullName ?? "کاربر انتخاب‌شده")
    : null;

  return (
    <div className="relative space-y-1.5" ref={containerRef}>
      <label className="block text-sm font-medium text-base-content">
        {label}
      </label>

      <input type="hidden" {...register(name)} />

      <button
        type="button"
        className={`input-saas input flex w-full items-center justify-between gap-2 text-start ${
          error ? "border-error" : ""
        }`}
        onClick={() => setOpen((p) => !p)}
      >
        <span className="truncate">
          {displayLabel ?? placeholder}
        </span>
        <ChevronDown
          className={`h-4 w-4 shrink-0 opacity-50 transition-transform duration-200 ${
            open ? "rotate-180" : ""
          }`}
        />
      </button>

      {open && (
        <div className="absolute z-50 mt-1 w-full space-y-2 rounded-2xl border border-base-300 bg-base-100 p-2 shadow-xl">
          <input
            autoFocus
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="جستجوی کاربر..."
            className="input-saas input w-full pl-10"
          />

          <ul
            ref={listRef}
            onScroll={handleScroll}
            className="max-h-60 overflow-y-auto"
          >
            {isLoading && (
              <li className="p-3 text-center text-sm opacity-60">
                در حال بارگذاری...
              </li>
            )}
            {!isLoading && users.length === 0 && (
              <li className="p-3 text-center text-sm opacity-60">
                کاربری یافت نشد
              </li>
            )}
            {users.map((user) => (
              <li key={user.id}>
                <button
                  type="button"
                  className={`w-full rounded-xl p-2.5 text-start transition-all duration-200 hover:bg-primary/10 ${
                    selectedId === user.id ? "bg-primary/15" : ""
                  }`}
                  onClick={() => handleSelect(user.id, user.fullName)}
                >
                  <span className="block text-sm font-medium">
                    {user.fullName}
                  </span>
                  <span className="block text-xs opacity-60">{user.email}</span>
                </button>
              </li>
            ))}
            {isFetchingNextPage && (
              <li className="p-2 text-center text-sm opacity-60">
                در حال بارگذاری بیشتر...
              </li>
            )}
          </ul>
        </div>
      )}

      {error && <p className="text-sm text-error">{error.message}</p>}
    </div>
  );
}
