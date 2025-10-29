import type { AuthUser, SystemResource, UserRead } from '../interfaces';
import { PermissionsMap } from './PermissionsMap';

interface EvalPermissions {
  username: string;
  permissions: SystemResource[];
}

function getUserPermissions(authUser: EvalPermissions): Set<string> {
  return new Set(authUser.permissions.map((p) => p.name));
}

export function isRootUser(authUser: EvalPermissions): boolean {
  return getUserPermissions(authUser).has(PermissionsMap.ROOT);
}

export function hasPermission(
  authUser: EvalPermissions,
  permissionName: string
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

export function canEditPermissions(
  authUser: AuthUser,
  targetUser?: UserRead
): boolean {
  const authPermissions = getUserPermissions(authUser);
  const isUserTeam = authPermissions.has(PermissionsMap.USERS);
  const isTargetRoot =
    targetUser && getUserPermissions(targetUser).has(PermissionsMap.ROOT);

  if (isRootUser(authUser)) return true;
  if (isUserTeam && !isTargetRoot) return true;

  return false;
}

export function filterAssignablePermissions(
  authUser: AuthUser,
  allPermissions: SystemResource[]
): SystemResource[] {
  if (isRootUser(authUser)) return allPermissions;
  const isUserTeam = getUserPermissions(authUser).has(PermissionsMap.USERS);
  if (isUserTeam)
    return allPermissions.filter((p) => p.name !== PermissionsMap.ROOT);
  return [];
}
