import { Bell, LogOutIcon, Menu, User, UserCircle } from "lucide-react";
import ThemeToggle from "../../shared/utils/ThemeToggle";
import { useAuth } from "../../shared/hooks/useAuth";
import { Link, useLocation, useNavigate } from "react-router-dom";

const pageTitles: Record<string, string> = {
  "/": "داشبورد",
  "/users": "مشتریان",
  "/projects": "پروژه‌ها",
  "/errors": "مدیریت خطا",
  "/profile": "پروفایل",
};

export default function Navbar() {
  const { logout, user } = useAuth();
  const navigate = useNavigate();
  const { pathname } = useLocation();

  const title =
    pageTitles[pathname] ??
    Object.entries(pageTitles).find(
      ([path]) => path !== "/" && pathname.startsWith(path),
    )?.[1] ??
    "پنل دوپالس";

  const handleLogout = () => {
    logout();
    navigate("/login", { replace: true });
  };

  return (
    <nav className="saas-chrome sticky top-0 z-30 flex h-[70px] w-full items-center gap-3 px-4 shadow-md sm:gap-4 sm:px-6">
      {/* Menu */}
      <label
        htmlFor="my-drawer-4"
        className="flex h-11 w-11 items-center justify-center rounded-xl transition-all duration-200 hover:bg-white/15 active:scale-[0.98] dark:hover:bg-black/10 lg:hidden"
        aria-label="باز کردن منو"
      >
        <Menu size={20} className="text-white dark:text-[#14532d]" />
      </label>

      {/* Title */}
      <div className="min-w-0">
        <h1 className="truncate text-base font-semibold tracking-tight sm:text-lg">
          {title}
        </h1>
        <p className="saas-chrome-muted hidden text-xs sm:block">
          پنل مدیریت دوپالس
        </p>
      </div>

      {/* Search */}
      {/* <div className="mx-auto hidden max-w-md flex-1 md:block">
        <label className="relative block">
          <Search
            size={16}
            className="saas-chrome-muted pointer-events-none absolute top-1/2 right-3 -translate-y-1/2"
          />
          <input
            type="search"
            placeholder="جستجو..."
            className="w-full rounded-xl border-0 bg-white/20 py-2.5 pr-10 pl-4 text-sm placeholder:text-inherit/50 outline-none transition-all duration-200 focus:bg-white/30 focus:ring-2 focus:ring-white/40"
            style={{ color: "var(--chrome-content)" }}
          />
        </label>
      </div> */}

      {/* Actions */}
      <div className="ms-auto flex items-center gap-2">
        {/* Notifications */}
        <button
          type="button"
          aria-label="اعلان‌ها"
          className="relative flex h-11 w-11 items-center justify-center rounded-xl transition-all duration-200 hover:bg-white/15 active:scale-95 dark:hover:bg-black/10"
        >
          <Bell size={19} className="text-white dark:text-[#14532d]" />

          <span className="absolute top-2 left-2 h-2.5 w-2.5 rounded-full bg-red-500 ring-2 ring-white dark:ring-[#4ade80]" />
        </button>

        {/* Theme */}
        <div className="flex h-11 w-11 items-center justify-center rounded-xl transition-all duration-200 hover:bg-white/15 dark:hover:bg-black/10">
          <ThemeToggle chrome={true} />
        </div>

        {/* Profile */}
        <div className="dropdown dropdown-end">
          <button
            tabIndex={0}
            type="button"
            className="flex h-11 items-center gap-3 rounded-xl px-3 transition-all duration-200 hover:bg-white/15 active:scale-[0.98] dark:hover:bg-black/10"
          >
            <div className="flex h-9 w-9 items-center justify-center rounded-full bg-white/20 backdrop-blur-sm dark:bg-black/10">
              <UserCircle
                size={20}
                className="text-white dark:text-[#14532d]"
              />
            </div>

            <span className="hidden max-w-[140px] truncate text-sm font-medium text-white dark:text-[#14532d] lg:inline">
              {user?.fullName ?? "کاربر"}
            </span>
          </button>

          <ul
            tabIndex={0}
            className="dropdown-content menu z-50 mt-3 w-56 rounded-xl border border-base-300 bg-base-100 p-2 shadow-2xl"
          >
            <li className="menu-title px-3 pt-2 text-xs">حساب کاربری</li>

            <li>
              <Link
                to="/profile"
                className="rounded-lg transition-colors hover:bg-base-200"
              >
                <User size={16} />
                پروفایل
              </Link>
            </li>

            <li>
              <button
                type="button"
                onClick={handleLogout}
                className="rounded-lg text-error transition-colors hover:bg-error/10"
              >
                <LogOutIcon size={16} />
                خروج از حساب
              </button>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
}
