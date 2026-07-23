import {
  BoxesIcon,
  LayoutDashboardIcon,
  TriangleAlert,
  UserCog,
  Zap,
} from "lucide-react";
import type { MenuItem } from "../../shared/types/Components/MenuItem";
import MenuItemComponent from "../../shared/components/MenuItem";
import { useAuth } from "../../shared/hooks/useAuth";

export default function Sidebar() {
  const { user } = useAuth();

  const menuItems: MenuItem[] = [
    {
      title: "داشبورد",
      path: "/",
      icon: LayoutDashboardIcon,
      hasAccess: true,
    },
    {
      title: "مشتریان",
      path: "/users",
      icon: UserCog,
      hasAccess: user?.isAdmin,
    },
    {
      title: "پروژه‌ها",
      path: "/projects",
      icon: BoxesIcon,
      hasAccess: true,
    },
    {
      title: "مدیریت خطا",
      path: "/errors",
      icon: TriangleAlert,
      hasAccess: true,
    },
  ];

  return (
    <div className="drawer-side z-40 is-drawer-close:overflow-visible">
      <label
        htmlFor="my-drawer-4"
        aria-label="بستن منو"
        className="drawer-overlay"
      />

      <aside className="saas-chrome flex min-h-full flex-col shadow-xl is-drawer-close:w-[4.5rem] is-drawer-open:w-64 transition-[width] duration-200">
        {/* Brand */}
        <div className="flex items-center gap-3 px-5 py-6">
          <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-white/25 dark:bg-black/15">
            <Zap size={20} className="saas-chrome-icon" />
          </div>
          <div className="is-drawer-close:hidden min-w-0">
            <p className="truncate text-base font-bold tracking-tight">
              دوپالس
            </p>
            <p className="saas-chrome-muted truncate text-xs">Admin Panel</p>
          </div>
        </div>

        <div className="mx-4 mb-4 h-px bg-current/15" />

        {/* Menu */}
        <ul className="menu w-full grow gap-1 px-3 py-1">
          {menuItems.map(
            (item) =>
              item.hasAccess && (
                <MenuItemComponent key={item.path} item={item} />
              ),
          )}
        </ul>

        {/* Footer */}
        <div className="saas-chrome-muted is-drawer-close:hidden px-5 py-4 text-xs">
          © {new Date().getFullYear()} DevPulse
        </div>
      </aside>
    </div>
  );
}
