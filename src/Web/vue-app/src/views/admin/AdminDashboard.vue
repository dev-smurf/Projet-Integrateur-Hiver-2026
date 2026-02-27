<template>
  <div class="admin-dashboard">
    <!-- Header -->
    <div class="dashboard-header">
      <h1>{{ t('pages.admin.dashboard.title') }}</h1>
      <p class="subtitle">{{ t('pages.admin.dashboard.subtitle') }}</p>
    </div>

    <!-- Stats Cards Section -->
    <div class="stats-section">
      <div class="stat-card stat-card--modules">
        <div class="stat-icon">📚</div>
        <div class="stat-content">
          <h3>{{ t('pages.admin.dashboard.stats.modules') }}</h3>
          <div class="stat-number">{{ activityStore.stats.totalModules }}</div>
          <p class="stat-label">{{ t('pages.admin.dashboard.stats.modulesLabel') }}</p>
        </div>
      </div>

      <div class="stat-card stat-card--members">
        <div class="stat-icon">👥</div>
        <div class="stat-content">
          <h3>{{ t('pages.admin.dashboard.stats.members') }}</h3>
          <div class="stat-number">{{ activityStore.stats.totalMembers }}</div>
          <p class="stat-label">{{ t('pages.admin.dashboard.stats.membersLabel') }}</p>
        </div>
      </div>

      <div class="stat-card stat-card--activities">
        <div class="stat-icon">🔄</div>
        <div class="stat-content">
          <h3>{{ t('pages.admin.dashboard.stats.activities') }}</h3>
          <div class="stat-number">{{ activityStore.stats.recentActivities }}</div>
          <p class="stat-label">{{ t('pages.admin.dashboard.stats.activitiesLabel') }}</p>
        </div>
      </div>

      <div class="stat-card stat-card--active">
        <div class="stat-icon">🟢</div>
        <div class="stat-content">
          <h3>{{ t('pages.admin.dashboard.stats.activeNow') }}</h3>
          <div class="stat-number">{{ activityStore.stats.activeNow }}</div>
          <p class="stat-label">{{ t('pages.admin.dashboard.stats.activeNowLabel') }}</p>
        </div>
      </div>
    </div>

    <!-- Quick Actions Section -->
    <div class="quick-actions">
      <router-link :to="{ name: 'admin-modules' }" class="action-card">
        <span class="action-icon">📖</span>
        <span class="action-label">{{ t('pages.admin.dashboard.actions.manageModules') }}</span>
      </router-link>
      <router-link :to="{ name: 'admin-members' }" class="action-card">
        <span class="action-icon">👤</span>
        <span class="action-label">{{ t('pages.admin.dashboard.actions.manageMembers') }}</span>
      </router-link>
      <router-link :to="{ name: 'admin-account' }" class="action-card">
        <span class="action-icon">⚙️</span>
        <span class="action-label">{{ t('pages.admin.dashboard.actions.settings') }}</span>
      </router-link>
    </div>

    <!-- Recent Activities Section -->
    <div class="content-grid content-grid--subpage">
      <div class="content-grid__header">
        <h2>{{ t('pages.admin.dashboard.recentActivities.title') }}</h2>
      </div>

      <!-- Filters & Search -->
      <div class="activities-controls">
        <div class="search-bar">
          <input
            v-model="activityStore.searchQuery"
            type="text"
            :placeholder="t('pages.admin.dashboard.recentActivities.placeholder')"
            @input="activityStore.setSearchQuery(($event.target as HTMLInputElement).value)"
            class="search-input"
          />
          <button class="search-btn">🔍</button>
        </div>

        <div class="filter-group">
          <button
            class="filter-btn"
            :class="{ active: !activityStore.selectedRole }"
            @click="activityStore.setSelectedRole(null)"
          >
            {{ t('pages.admin.dashboard.recentActivities.allRoles') }}
          </button>
          <button
            v-for="role in activityStore.uniqueRoles"
            :key="role"
            class="filter-btn"
            :class="{ active: activityStore.selectedRole === role }"
            @click="activityStore.setSelectedRole(role)"
          >
            {{ role }}
          </button>
        </div>
      </div>

      <!-- Loader -->
      <Loader v-if="activityStore.loading" />

      <!-- Error Message -->
      <div v-if="activityStore.error" class="error-message">
        {{ activityStore.error }}
      </div>

      <!-- Activities List (Card Style) -->
      <div v-if="!activityStore.loading && activityStore.filteredActivities.length > 0" class="activities-list">
        <div v-for="activity in activityStore.filteredActivities" :key="activity.id" class="activity-card">
          <div class="activity-avatar">
            <img :src="activity.avatar" :alt="activity.username" class="avatar-img" />
            <span
              v-if="activity.status"
              class="status-indicator"
              :class="`status--${activity.status}`"
            ></span>
          </div>

          <div class="activity-details">
            <div class="activity-header">
              <h4 class="activity-name">{{ activity.username }}</h4>
              <span class="activity-role">{{ activity.role }}</span>
            </div>
            <div class="activity-meta">
              <span class="activity-module">{{ activity.module }}</span>
            </div>
            <p class="activity-action">{{ activity.action }}</p>
          </div>

          <div class="activity-timestamp">
            <span class="time-label">{{ formatTime(activity.timestamp) }}</span>
          </div>

          <!-- Optional: Action indicator checkbox -->
          <div class="activity-check">
            <input type="checkbox" :checked="activity.status === 'completed'" disabled />
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else-if="!activityStore.loading" class="empty-message">
        {{ t('pages.admin.dashboard.recentActivities.empty') }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useI18n } from 'vue3-i18n';
import { useActivityStore } from '@/stores/activityStore';
import Loader from '@/components/layouts/items/Loader.vue';
import '../../sass/blocks/AdminDashboard.scss';

const { t } = useI18n();
const activityStore = useActivityStore();

function formatTime(date: Date): string {
  const now = new Date();
  const diff = (now.getTime() - date.getTime()) / 1000;

  if (diff < 60) return `${Math.floor(diff)}s ${t('pages.admin.dashboard.recentActivities.ago')}`;
  if (diff < 3600) return `${Math.floor(diff / 60)}m ${t('pages.admin.dashboard.recentActivities.ago')}`;
  if (diff < 86400) return `${Math.floor(diff / 3600)}h ${t('pages.admin.dashboard.recentActivities.ago')}`;
  return `${Math.floor(diff / 86400)}d ${t('pages.admin.dashboard.recentActivities.ago')}`;
}

onMounted(async () => {
  await activityStore.loadStats();
  await activityStore.loadActivities();
});
</script>
