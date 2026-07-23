import { Modal } from "../../shared/components/Modal";
import {
  AlertTriangle,
  Copy,
  Clock,
  Hash,
  Globe,
  Monitor,
  Shield,
  User,
  Terminal,
  Code,
  Image as ImageIcon,
  ExternalLink,
} from "lucide-react";
import { useError } from "../../shared/hooks/useErrors";

interface Props {
  errorId?: string | null;
  open: boolean;
  onClose: () => void;
}

export default function ShowErrorDetailModal({
  open,
  errorId,
  onClose,
}: Props) {
  const {
    data: error,
    isLoading,
    isError,
    error: queryError,
  } = useError(errorId ?? "");

  // اگر errorId خالی بود یا مودال بسته است، هیچ چیزی رندر نکن
  if (!errorId || !open) return null;

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString("fa-IR", {
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text).catch((err) => {
      console.error("Failed to copy: ", err);
    });
  };

  return (
    <Modal open={open} onClose={onClose} title="جزئیات خطا" size="xlg">
      <div className="space-y-6 p-1">
        {/* حالت Loading */}
        {isLoading && (
          <div className="flex items-center justify-center py-20">
            <span className="loading loading-spinner loading-lg text-primary"></span>
            <span className="mr-3 text-base-content/60">
              در حال بارگذاری...
            </span>
          </div>
        )}

        {/* حالت Error */}
        {isError && (
          <div className="alert alert-error">
            <AlertTriangle className="h-6 w-6" />
            <span>
              خطا در دریافت اطلاعات:{" "}
              {queryError?.message || "لطفاً دوباره تلاش کنید"}
            </span>
          </div>
        )}

        {/* نمایش داده‌ها */}
        {error && !isLoading && !isError && (
          <>
            {/* 1. بخش اصلی: نوع خطا و پیام */}
            <div className="space-y-4">
              <div className="flex items-start justify-between gap-4">
                <div className="space-y-1">
                  <h3 className="text-xl font-bold text-base-content flex items-center gap-2">
                    <AlertTriangle className="h-5 w-5 text-error" />
                    {error.exceptionType}
                  </h3>
                  <p className="text-sm text-base-content/60 flex items-center gap-1.5 flex-wrap">
                    <span className="font-medium text-base-content">
                      {error.projectName}
                    </span>
                    <span className="text-base-content/40">•</span>
                    <Clock className="h-3.5 w-3.5" />
                    {formatDate(error.createdAt)}
                  </p>
                </div>
              </div>

              <div className="alert alert-error shadow-sm">
                <AlertTriangle className="h-6 w-6 shrink-0" />
                <span className="break-all font-medium leading-relaxed">
                  {error.message}
                </span>
              </div>
            </div>

            {/* 2. بخش متادیتاها (Grid) */}
            <div>
              <div className="divider my-2 text-base-content/50 text-sm">
                اطلاعات درخواست
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-6 gap-y-5">
                <DetailItem
                  icon={<Hash className="h-3.5 w-3.5" />}
                  label="شناسه خطا"
                  value={error.id}
                />
                <DetailItem
                  icon={<Terminal className="h-3.5 w-3.5" />}
                  label="متد (Method)"
                  value={error.method}
                />
                <DetailItem
                  icon={<Globe className="h-3.5 w-3.5" />}
                  label="آدرس (URL)"
                  value={error.url}
                  onCopy={() => copyToClipboard(error.url)}
                />
                {error.queryString && (
                  <DetailItem
                    icon={<Code className="h-3.5 w-3.5" />}
                    label="Query String"
                    value={error.queryString}
                    onCopy={() => copyToClipboard(error.queryString!)}
                  />
                )}
                <DetailItem
                  icon={<Shield className="h-3.5 w-3.5" />}
                  label="آدرس IP"
                  value={error.ipAddress ?? "نامشخص"}
                />
                <DetailItem
                  icon={<Monitor className="h-3.5 w-3.5" />}
                  label="مرورگر"
                  value={error.browser ?? "نامشخص"}
                />
                {error.projectId && (
                  <DetailItem
                    icon={<User className="h-3.5 w-3.5" />}
                    label="شناسه پروژه"
                    value={error.projectId}
                  />
                )}
              </div>
            </div>

            {/* 3. بخش Request Body */}
            {error.requestBody && (
              <div className="space-y-2">
                <div className="flex items-center justify-between">
                  <label className="text-sm font-bold text-base-content/80 flex items-center gap-2">
                    <Code className="h-4 w-4" />
                    Request Body
                  </label>
                  <button
                    className="btn btn-xs btn-ghost gap-1.5 text-base-content/70 hover:text-base-content"
                    onClick={() => copyToClipboard(error.requestBody)}
                    title="کپی در کلیپ‌بورد"
                  >
                    <Copy className="h-3.5 w-3.5" />
                    کپی
                  </button>
                </div>
                <pre
                  className="bg-base-200 text-base-content/80 text-xs font-mono p-4 rounded-box overflow-x-auto whitespace-pre-wrap break-words max-h-48 overflow-y-auto border border-base-300 shadow-inner"
                  dir="ltr"
                >
                  {error.requestBody}
                </pre>
              </div>
            )}

            {/* 4. بخش Stack Trace */}
            <div className="space-y-2">
              <div className="flex items-center justify-between">
                <label className="text-sm font-bold text-base-content/80 flex items-center gap-2">
                  <Terminal className="h-4 w-4" />
                  Stack Trace
                </label>
                <button
                  className="btn btn-xs btn-ghost gap-1.5 text-base-content/70 hover:text-base-content"
                  onClick={() => copyToClipboard(error.stackTrace)}
                  title="کپی در کلیپ‌بورد"
                >
                  <Copy className="h-3.5 w-3.5" />
                  کپی
                </button>
              </div>
              <pre
                className="bg-base-200 text-base-content/80 text-xs font-mono p-4 rounded-box overflow-x-auto whitespace-pre-wrap break-words max-h-64 overflow-y-auto border border-base-300 shadow-inner"
                dir="ltr"
              >
                {error.stackTrace}
              </pre>
            </div>

            {/* 5. بخش Screenshot */}
            {error.screenshot && (
              <div className="space-y-2">
                <label className="text-sm font-bold text-base-content/80 flex items-center gap-2">
                  <ImageIcon className="h-4 w-4" />
                  اسکرین‌شات خطا
                </label>
                <div className="relative rounded-box border border-base-300 bg-base-200 overflow-hidden group">
                  <img
                    src={error.screenshot.s3Url}
                    alt="Error Screenshot"
                    className="w-full h-auto max-h-96 object-contain transition-transform duration-300 group-hover:scale-[1.02]"
                  />
                  <a
                    href={error.screenshot.s3Url}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="absolute top-3 left-3 btn btn-sm btn-primary gap-1.5 shadow-md opacity-90 hover:opacity-100"
                  >
                    <ExternalLink className="h-4 w-4" />
                    مشاهده تمام‌صفحه
                  </a>
                </div>
                <p className="text-xs text-base-content/50 text-left" dir="ltr">
                  Size: {(error.screenshot.sizeInBytes / 1024).toFixed(2)} KB •
                  Type: {error.screenshot.contentType}
                </p>
              </div>
            )}
          </>
        )}
      </div>
    </Modal>
  );
}

// ────────────────────────────────────────────────────────────────
// Sub-component: نمایش تمیز و یکپارچه هر آیتم متادیتا
// ────────────────────────────────────────────────────────────────
function DetailItem({
  label,
  value,
  onCopy,
  icon,
}: {
  label: string;
  value: string;
  onCopy?: () => void;
  icon?: React.ReactNode;
}) {
  return (
    <div className="flex flex-col gap-1.5">
      <span className="text-xs font-semibold text-base-content/60 uppercase tracking-wide flex items-center gap-1.5">
        {icon && <span className="text-base-content/40">{icon}</span>}
        {label}
      </span>
      <div className="flex items-center gap-2 group">
        <span
          className="text-sm font-medium text-base-content break-all font-mono bg-base-200/50 px-2.5 py-1.5 rounded-md w-full border border-base-300/50 transition-colors group-hover:border-base-content/20"
          dir="ltr"
        >
          {value}
        </span>
        {onCopy && (
          <button
            className="btn btn-circle btn-ghost btn-xs opacity-0 group-hover:opacity-100 transition-all shrink-0"
            onClick={onCopy}
            title="کپی"
          >
            <Copy className="h-3.5 w-3.5" />
          </button>
        )}
      </div>
    </div>
  );
}
