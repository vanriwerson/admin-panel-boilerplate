import { useContext } from 'react';
import { AuthContext } from '../contexts';
import type { AuthContextType } from '../interfaces';

export function useAuth(): AuthContextType {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within an AuthProvider');
  return context;
}
