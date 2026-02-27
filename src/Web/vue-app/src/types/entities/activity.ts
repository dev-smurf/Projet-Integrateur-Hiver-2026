export interface Activity {
  id: string;
  username: string;
  role: string;
  module: string;
  timestamp: Date;
  action: string;
  avatar?: string;
  status?: 'pending' | 'completed' | 'in-progress';
}

export interface DashboardStats {
  totalModules: number;
  totalMembers: number;
  recentActivities: number;
  activeNow: number;
}
