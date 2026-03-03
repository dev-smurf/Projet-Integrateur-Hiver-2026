<template>
  <div>
    <h1 class="text-2xl font-bold text-gray-900 mb-2">
      {{ $t('pages.dashboard.welcome') }}, {{ personStore.person.fullName || userStore.user.fullName }}
    </h1>

  </div>
</template>

<script lang="ts" setup>
import {useUserStore} from "@/stores/userStore";
import {usePersonStore} from "@/stores/personStore";

const userStore = useUserStore();
const personStore = usePersonStore();
</script>
