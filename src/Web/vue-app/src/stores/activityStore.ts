import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import type { Activity, DashboardStats } from '@/types/entities/activity';
import { MOCK_ACTIVITIES, MOCK_DASHBOARD_STATS } from '@/services/mockActivitiesData';

const USE_MOCK_DATA = true; // Toggle to false when backend ready

export const useActivityStore = defineStore('activity', () => {
  const activities = ref<Activity[]>([]);
  const stats = ref<DashboardStats>(MOCK_DASHBOARD_STATS);
  const loading = ref(false);
  const error = ref<string | null>(null);
  const searchQuery = ref('');
  const selectedRole = ref<string | null>(null);

  const filteredActivities = computed(() => {
    return activities.value.filter((activity) => {
      const matchesSearch =
        activity.username.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
        activity.module.toLowerCase().includes(searchQuery.value.toLowerCase());
      const matchesRole = !selectedRole.value || activity.role === selectedRole.value;
      return matchesSearch && matchesRole;
    });
  });

  const uniqueRoles = computed(() => {
    return Array.from(new Set(activities.value.map((a) => a.role)));
  });

  async function loadActivities() {
    loading.value = true;
    error.value = null;
    try {
      if (USE_MOCK_DATA) {
        // Simulate API delay
        await new Promise((resolve) => setTimeout(resolve, 500));
        activities.value = MOCK_ACTIVITIES;
      } else {
        // When backend is ready:
        // const response = await apiService.get('/admin/activities');
        // activities.value = response.data;
        // stats.value = response.stats;
        activities.value = MOCK_ACTIVITIES;
      }
    } catch (err: any) {
      error.value = err.message || 'Failed to load activities';
    } finally {
      loading.value = false;
    }
  }

  async function loadStats() {
    try {
      if (USE_MOCK_DATA) {
        stats.value = MOCK_DASHBOARD_STATS;
      } else {
        // const response = await apiService.get('/admin/dashboard/stats');
        // stats.value = response.data;
        stats.value = MOCK_DASHBOARD_STATS;
      }
    } catch (err: any) {
      error.value = err.message || 'Failed to load stats';
    }
  }

  function setSearchQuery(query: string) {
    searchQuery.value = query;
  }

  function setSelectedRole(role: string | null) {
    selectedRole.value = role;
  }

  return {
    activities,
    stats,
    loading,
    error,
    searchQuery,
    selectedRole,
    filteredActivities,
    uniqueRoles,
    loadActivities,
    loadStats,
    setSearchQuery,
    setSelectedRole,
  };
});
