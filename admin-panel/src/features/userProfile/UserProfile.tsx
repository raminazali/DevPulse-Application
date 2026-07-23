import { Shield, Mail, User as UserIcon } from "lucide-react";
import { useAuth } from "../../shared/hooks/useAuth";

export default function UserProfile() {
  const { user } = useAuth();

  if (!user) {
    return (
      <div className="flex min-h-40 items-center justify-center">
        <span className="loading loading-spinner loading-md text-primary" />
      </div>
    );
  }

  return (
    <div className="saas-page max-w-2xl">
      <div className="saas-page-header">
        <div>
          <h2 className="saas-page-title">پروفایل</h2>
          <p className="saas-page-desc">اطلاعات حساب کاربری شما</p>
        </div>
      </div>

      <div className="saas-card space-y-6">
        <div className="flex items-center gap-4">
          <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-primary/15 text-primary">
            <UserIcon size={28} strokeWidth={1.5} />
          </div>
          <div>
            <p className="text-lg font-semibold">{user.fullName}</p>
            <p className="saas-card-subtitle">{user.email}</p>
          </div>
        </div>

        <div className="h-px bg-base-300" />

        <div className="space-y-4">
          <Field
            icon={<UserIcon size={16} />}
            label="نام کامل"
            value={user.fullName}
          />
          <Field
            icon={<Mail size={16} />}
            label="ایمیل"
            value={user.email}
          />
          <Field
            icon={<Shield size={16} />}
            label="سطح دسترسی"
            value={user.isAdmin ? "مدیر سیستم" : "کاربر عادی"}
          />
        </div>

        <div className="flex justify-end pt-2">
          <button type="button" disabled className="btn-saas btn btn-sm opacity-50">
            ویرایش اطلاعات
          </button>
        </div>
      </div>
    </div>
  );
}

function Field({
  label,
  value,
  icon,
}: {
  label: string;
  value: string;
  icon: React.ReactNode;
}) {
  return (
    <div className="space-y-1.5">
      <label className="flex items-center gap-1.5 text-sm font-medium text-base-content">
        <span className="text-primary">{icon}</span>
        {label}
      </label>
      <input
        type="text"
        value={value}
        disabled
        className="input-saas input w-full opacity-80"
      />
    </div>
  );
}
