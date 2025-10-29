import type { IconDefinition } from '@fortawesome/fontawesome-svg-core';

export interface MenuItem {
  label: string;
  route: string;
  icon: IconDefinition;
  permission?: string;
}
