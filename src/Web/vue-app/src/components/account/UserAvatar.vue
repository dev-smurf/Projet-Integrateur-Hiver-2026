<template>
  <RouterLink :to="t('routes.account.path')" class="user-avatar">
    <div class="user-avatar__circle">
      <span class="user-avatar__initials">{{ initials }}</span>
    </div>
    <p class="user-avatar__name">{{ personStore.person.fullName }}</p>
  </RouterLink>
</template>

<script lang="ts" setup>
import {computed} from "vue";
import {usePersonStore} from "@/stores/personStore";
import {useI18n} from "vue3-i18n";

const personStore = usePersonStore();
const {t} = useI18n();

const initials = computed(() => {
  const first = personStore.person.firstName ?? '';
  const last = personStore.person.lastName ?? '';
  return (first.charAt(0) + last.charAt(0)).toUpperCase() || '?';
});
</script>
