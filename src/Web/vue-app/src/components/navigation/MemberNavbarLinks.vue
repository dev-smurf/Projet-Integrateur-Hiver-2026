<template>
  <RouterLink :to="t('routes.account.path')" class="nav-link">
    <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
      <path stroke-linecap="round" stroke-linejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
    </svg>
    {{ t('routes.account.name') }}
  </RouterLink>
  <RouterLink :to="t('routes.books.path')" class="nav-link">
    <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
      <path stroke-linecap="round" stroke-linejoin="round" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
    </svg>
    {{ t('routes.books.name') }}
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
