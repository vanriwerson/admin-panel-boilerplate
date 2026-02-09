import type { SystemResourceOption } from '../interfaces';

export function mapSystemResourcesToFormValue(
  permissions: SystemResourceOption[] | undefined
): number[] {
  if (!permissions) return [];
  return permissions
    .map((res) => res.id)
    .filter((id): id is number => id !== undefined);
}
