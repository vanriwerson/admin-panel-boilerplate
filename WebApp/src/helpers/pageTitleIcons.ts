import type { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import { menuItems } from './menuItems';

export const pageTitleIcons = menuItems.reduce((acc, item) => {
  const key = item.permission ?? item.route.replace('/', '');
  acc[key] = item.icon;
  return acc;
}, {} as Record<string, IconDefinition>);
