<template>
  <RouterLink :to="t('routes.admin.path') + '/' + t('routes.admin.children.members.path')" class="nav-link" :title="t('routes.admin.name')">
    <svg width="20" height="20" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
      <path stroke-linecap="round" stroke-linejoin="round" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
    </svg>
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
  justify-content: center;
  padding: 6px;
  border-radius: 6px;
  color: $color-grey !important;
  text-decoration: none !important;
  transition: color 0.15s, background-color 0.15s;

  &:hover {
    color: $color-white !important;
  }

  &.router-link-active {
    color: $color-white !important;
  }

  svg {
    flex-shrink: 0;
  }
}
</style>
