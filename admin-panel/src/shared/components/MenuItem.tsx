import { NavLink } from "react-router-dom";
import { ChevronDown } from "lucide-react";
import type { MenuItemProps } from "../types/Components/MenuItem";

export default function MenuItemComponent({ item }: MenuItemProps) {
  const Icon = item.icon;

  if (item.children?.length) {
    return (
      <li>
        <details className="group [&_summary::after]:hidden">
          <summary
            className="
              is-drawer-close:tooltip is-drawer-close:tooltip-left
              flex cursor-pointer items-center gap-3 rounded-xl px-3 py-3
              transition-all duration-200 hover:bg-white/10 dark:hover:bg-black/10
            "
            data-tip={item.title}
          >
            {Icon && <Icon size={20} strokeWidth={1.75} className="shrink-0" />}
            <span className="is-drawer-close:hidden text-sm font-medium">
              {item.title}
            </span>
            <ChevronDown
              size={16}
              className="mr-auto is-drawer-close:hidden transition-transform duration-200 group-open:rotate-180"
            />
          </summary>

          <ul className="is-drawer-close:hidden ms-2 mt-1 space-y-1 border-r border-current/15 pr-2">
            {item.children.map((child) => (
              <MenuItemComponent key={child.path} item={child} />
            ))}
          </ul>
        </details>
      </li>
    );
  }

  return (
    <li>
      <NavLink
        to={item.path}
        end={item.path === "/"}
        data-tip={item.title}
        className={({ isActive }) =>
          [
            "is-drawer-close:tooltip is-drawer-close:tooltip-left",
            "flex items-center gap-3 rounded-xl px-3 py-3 text-sm",
            "transition-all duration-200",
            isActive
              ? "font-semibold shadow-sm"
              : "font-medium hover:bg-white/10 dark:hover:bg-black/10",
          ].join(" ")
        }
        style={({ isActive }) =>
          isActive
            ? {
                backgroundColor: "var(--menu-active-bg)",
                color: "var(--menu-active-text)",
                border:
                  "1px solid var(--menu-active-border, transparent)",
                boxShadow: "var(--menu-active-shadow)",
              }
            : undefined
        }
      >
        {Icon && <Icon size={20} strokeWidth={1.75} className="shrink-0" />}
        <span className="is-drawer-close:hidden">{item.title}</span>
      </NavLink>
    </li>
  );
}
