import type { AuthUser, SystemResource, UserRead } from '../interfaces';
import { PermissionsMap } from './PermissionsMap';

interface EvalPermissions {
  username: string;
  permissions: SystemResource[];
}

function getUserPermissions(authUser: EvalPermissions): Set<string> {
  return new Set(authUser.permissions.map((p) => p.name));
}

function isRootUser(authUser: EvalPermissions): boolean {
  return getUserPermissions(authUser).has(PermissionsMap.ROOT);
}

export function hasPermission(
  authUser: EvalPermissions,
  permissionName: PermissionName
): boolean {
  const permissions = getUserPermissions(authUser);
  return isRootUser(authUser) || permissions.has(permissionName);
}

export function canEditPassword(
  authUser: AuthUser,
  targetUser?: UserRead
): boolean {
  const authPermissions = getUserPermissions(authUser);
  const isUserTeam = authPermissions.has(PermissionsMap.USERS);
  const isEditingSelf = targetUser && authUser.username === targetUser.username;
  const isTargetRoot =
    targetUser && getUserPermissions(targetUser).has(PermissionsMap.ROOT);

  if (isRootUser(authUser)) return true;
  if (isUserTeam && !isTargetRoot) return true;
  return Boolean(isEditingSelf);
}
