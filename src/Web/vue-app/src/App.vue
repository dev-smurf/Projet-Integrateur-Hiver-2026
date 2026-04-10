<template>
  <AuthenticationLayout v-if="!userStore.user.email || isAuthenticationPath"/>
  <DashboardLayout v-else/>
</template>

<script lang="ts" setup>
import {computed, onMounted, watch} from "vue";
import {useRouter} from "vue-router";
import {useUserStore} from "@/stores/userStore";
import {usePersonStore} from "@/stores/personStore";
import {useApiStore} from "@/stores/apiStore";
import AuthenticationLayout from "@/components/layouts/AuthenticationLayout.vue";
import DashboardLayout from "@/components/layouts/DashboardLayout.vue";
import {useUserService} from "@/inversify.config";

const router = useRouter();
const userStore = useUserStore();
const personStore = usePersonStore();
const apiStore = useApiStore();
const userService = useUserService();

const authenticationRoutes = ['login', 'forgotPassword', 'resetPassword']
let isAuthenticationPath = computed(() => {
  return authenticationRoutes.includes(router.currentRoute.value.name as string)
});

async function resetSessionAndRedirect() {
  userStore.reset();
  personStore.reset();

  if (!isAuthenticationPath.value) {
    await router.push({ name: "login" });
  }
}

async function syncCurrentUser() {
  if (isAuthenticationPath.value) {
    return;
  }

  try {
    const currentUser = await userService.getCurrentUser();

    if (!currentUser?.email) {
      await resetSessionAndRedirect();
      return;
    }

    userStore.setUser(currentUser);
  } catch {
    await resetSessionAndRedirect();
  }
}

watch(() => apiStore.needToLogout, async needToLogout => {
  if (!needToLogout) {
    return;
  }

  apiStore.setNeedToLogout(false);
  await resetSessionAndRedirect();
});

onMounted(syncCurrentUser);
</script>
