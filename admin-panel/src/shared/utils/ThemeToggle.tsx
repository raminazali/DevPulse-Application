import { Moon, Sun } from "lucide-react";
import { useState } from "react";

const LIGHT = "devpulse-light";
const DARK = "devpulse-dark";

function getInitialTheme() {
  if (typeof window === "undefined") return LIGHT;
  const saved = localStorage.getItem("theme");
  if (saved === DARK || saved === "dark") return DARK;
  if (saved === LIGHT || saved === "light") return LIGHT;
  return LIGHT;
}

export default function ThemeToggle({
  className = "",
  chrome = false,
}: {
  className?: string;
  /** When true, uses chrome-aware icon colors (navbar/sidebar). */
  chrome?: boolean;
}) {
  const [theme, setTheme] = useState(() => {
    const initial = getInitialTheme();
    document.documentElement.setAttribute("data-theme", initial);
    return initial;
  });

  const isLight = theme === LIGHT;

  const toggleTheme = () => {
    const next = isLight ? DARK : LIGHT;
    setTheme(next);
    localStorage.setItem("theme", next);
    document.documentElement.setAttribute("data-theme", next);
  };

  return (
    <button
      type="button"
      onClick={toggleTheme}
      className={
        chrome
          ? `
        flex items-center justify-center
        h-10 w-10
        rounded-xl
        bg-transparent
        hover:bg-transparent
        transition-all duration-200
        hover:scale-[1.05]
        active:scale-[0.95]
        ${className}
      `
          : `
        btn btn-ghost btn-square btn-sm
        rounded-xl
        transition-all duration-200
        hover:scale-[1.02]
        active:scale-[0.98]
        ${className}
      `
      }
      aria-label={isLight ? "حالت تاریک" : "حالت روشن"}
      title={isLight ? "حالت تاریک" : "حالت روشن"}
    >
      {isLight ? (
        <Sun size={18} className={chrome ? "text-black" : ""} />
      ) : (
        <Moon size={18} className={chrome ? "text-black" : ""} />
      )}
    </button>
  );
}
