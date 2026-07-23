import { useEffect, useRef } from "react";
import { createPortal } from "react-dom";
import { X } from "lucide-react";
import type { ModalProps } from "../types/Components/Modal";

export function Modal({
  open,
  title,
  onClose,
  children,
  footer,
  size = "md",
}: ModalProps) {
  const previousActiveElement = useRef<HTMLElement | null>(null);

  useEffect(() => {
    if (!open) return;

    previousActiveElement.current = document.activeElement as HTMLElement;

    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };

    document.body.style.overflow = "hidden";
    window.addEventListener("keydown", onKeyDown);

    return () => {
      document.body.style.overflow = "";
      window.removeEventListener("keydown", onKeyDown);
      previousActiveElement.current?.focus();
    };
  }, [open, onClose]);

  if (!open) return null;

  const sizeClass =
    size === "sm"
      ? "max-w-sm"
      : size === "md"
        ? "max-w-xl"
        : size === "lg"
          ? "max-w-4xl"
          : size === "xlg"
            ? "max-w-6xl"
            : "max-w-xl";

  return createPortal(
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div
        className="absolute inset-0 bg-black/40 backdrop-blur-[2px] transition-opacity duration-200"
        onClick={onClose}
        aria-hidden
      />

      <div
        role="dialog"
        aria-modal="true"
        aria-label={title}
        className={`relative w-full ${sizeClass} max-h-[90vh] overflow-hidden rounded-2xl border border-base-300 bg-base-100 shadow-2xl transition-all duration-200`}
      >
        <div className="flex items-center justify-between border-b border-base-300 px-5 py-4">
          <h2 className="text-base font-semibold tracking-tight">{title}</h2>
          <button
            type="button"
            className="btn btn-ghost btn-sm btn-square rounded-xl transition-all duration-200 hover:bg-base-200"
            onClick={onClose}
            aria-label="بستن"
          >
            <X size={18} />
          </button>
        </div>

        <div className="max-h-[calc(90vh-8rem)] overflow-y-auto p-5">
          {children}
        </div>

        {footer && (
          <div className="flex justify-end gap-2 border-t border-base-300 px-5 py-4">
            {footer}
          </div>
        )}
      </div>
    </div>,
    document.body,
  );
}
