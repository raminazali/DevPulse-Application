export type MenuItem = {
  title: string;
  path: string;
  icon?: React.ElementType;
  hasAccess?: boolean;
  children?: MenuItem[];
};

export type MenuItemProps = {
  item: MenuItem;
};
