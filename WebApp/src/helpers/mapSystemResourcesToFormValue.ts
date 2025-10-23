import type { SystemResource } from '../interfaces';

export function mapSystemResourcesToFormValue(
  permissions: SystemResource[] | undefined
): number[] {
  if (!permissions) return [];
  return permissions
    .map((res) => res.id)
    .filter((id): id is number => id !== undefined);
}
