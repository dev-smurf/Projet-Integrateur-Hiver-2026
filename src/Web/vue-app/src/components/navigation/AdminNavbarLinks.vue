<template>
  <RouterLink :to="t('routes.account.path')" class="nav-link">
    <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
      <path stroke-linecap="round" stroke-linejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
    </svg>
    {{ t('routes.account.name') }}
  </RouterLink>
  <RouterLink :to="t('routes.admin.path') + '/' + t('routes.admin.children.members.path')" class="nav-link">
    <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
      <path stroke-linecap="round" stroke-linejoin="round" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z" />
    </svg>
    {{ t('routes.admin.name') }}
  </RouterLink>
</template>

<script lang="ts" setup>
import { useI18n } from "vue3-i18n";
const {t} = useI18n()
</script>

<style scoped lang="scss">
@use "../../sass/tools" as *;

.nav-link {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 14px;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 600;
  color: $color-grey !important;
  text-decoration: none !important;
  white-space: nowrap;
  transition: color 0.15s, background-color 0.15s;

  &:hover {
    color: $color-white !important;
    background: $color-grey-darker !important;
  }

  &.router-link-active {
    color: $color-white !important;
    background: $color-grey-darker !important;
  }

  svg {
    flex-shrink: 0;
  }
}
</style>
