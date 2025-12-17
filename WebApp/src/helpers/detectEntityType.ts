const ENTITY_SIGNATURES = {
  user: ['id', 'username', 'email', 'fullName', 'permissions'],
  systemResource: ['name', 'exhibitionName'],
} as const;

type EntityType = keyof typeof ENTITY_SIGNATURES;

export function detectEntityType(
  payload: Record<string, unknown>
): EntityType | null {
  const payloadKeys = Object.keys(payload);

  for (const [entity, keys] of Object.entries(ENTITY_SIGNATURES)) {
    const matches = keys.every((key) => payloadKeys.includes(key));
    if (matches) return entity as EntityType;
  }

  return null;
}
