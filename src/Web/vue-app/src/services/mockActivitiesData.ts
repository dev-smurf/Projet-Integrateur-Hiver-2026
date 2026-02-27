import type { Activity, DashboardStats } from '@/types/entities/activity';

export const MOCK_ACTIVITIES: Activity[] = [
  {
    id: '1',
    username: 'Hernandez Lontai',
    role: 'Soumis',
    module: 'Module 3',
    timestamp: new Date(Date.now() - 8 * 60000), // 8 minutes ago
    action: 'El logre t y a 8 minutos',
    avatar: 'https://i.pravatar.cc/150?img=1',
    status: 'completed',
  },
  {
    id: '2',
    username: 'Bradley deid',
    role: 'Etudiant',
    module: 'Module 2',
    timestamp: new Date(Date.now() - 2 * 60000), // 2 minutes ago
    action: 'El logre',
    avatar: 'https://i.pravatar.cc/150?img=2',
    status: 'completed',
  },
  {
    id: '3',
    username: 'Orlane mang',
    role: 'Tuteur',
    module: 'Module 3',
    timestamp: new Date(Date.now() - 1 * 60000), // 1 minute ago
    action: 'El logre t y a 1 semaine',
    avatar: 'https://i.pravatar.cc/150?img=3',
    status: 'in-progress',
  },
  {
    id: '4',
    username: 'Sophie Lavoie',
    role: 'Soumis',
    module: 'Module 1',
    timestamp: new Date(Date.now() - 15 * 60000), // 15 minutes ago
    action: 'El logre t y a 15 minutos',
    avatar: 'https://i.pravatar.cc/150?img=4',
    status: 'completed',
  },
  {
    id: '5',
    username: 'Jean Dupont',
    role: 'Instructeur',
    module: 'Module 5',
    timestamp: new Date(Date.now() - 45 * 60000), // 45 minutes ago
    action: 'El logre t y a 45 minutos',
    avatar: 'https://i.pravatar.cc/150?img=5',
    status: 'pending',
  },
];

export const MOCK_DASHBOARD_STATS: DashboardStats = {
  totalModules: 42,
  totalMembers: 18,
  recentActivities: 156,
  activeNow: 7,
};
