export interface PermissionsContextProps {
  permissionsMap: Record<string, string>;
  loading: boolean;
  error: string | null;
  refreshPermissions: () => Promise<void>;
}
