import { createContext, useState, type ReactNode } from "react";
import type {
  AuthUser,
  MenuItem,
  PermissionsContextProps,
} from "../interfaces";
import { cleanStates, getPageTitleIcons, menuItems } from "../helpers";
import { hasPermission, isRootUser } from "../permissions/Rules";

const PermissionsContext = createContext<PermissionsContextProps | undefined>(
  undefined,
);
export default PermissionsContext;

export function PermissionsProvider({ children }: { children: ReactNode }) {
  const permissionsMap = cleanStates.initialPermissionsMap;
  const [loading, _setLoading] = useState(true);
  const [error, _setError] = useState<string | null>(null);

  const pageTitleIcons = getPageTitleIcons(menuItems);

  const getMenuItemsForUser = (authUser: AuthUser | null): MenuItem[] => {
    return menuItems.filter((item) => {
      if (!item.permission) return true;
      if (!authUser) return false;
      if (isRootUser(authUser)) return true;
      return hasPermission(authUser, item.permission);
    });
  };

  return (
    <PermissionsContext.Provider
      value={{
        permissionsMap,
        pageTitleIcons,
        menuItems,
        loading,
        error,
        getMenuItemsForUser,
      }}
    >
      {children}
    </PermissionsContext.Provider>
  );
}
