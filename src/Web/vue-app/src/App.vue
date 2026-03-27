<template>
  <AuthenticationLayout v-if="!userStore.user.email || isAuthenticationPath"/>
  <DashboardLayout v-else/>
</template>

<script lang="ts" setup>
import {computed, onMounted, watchEffect} from "vue";
import {useRouter} from "vue-router";
import {useUserStore} from "@/stores/userStore";
import AuthenticationLayout from "@/components/layouts/AuthenticationLayout.vue";
import DashboardLayout from "@/components/layouts/DashboardLayout.vue";
import {useUserService} from "@/inversify.config";
import {Role} from "@/types/enums";

const router = useRouter();
const userStore = useUserStore();
const userService = useUserService();

const authenticationRoutes = ['login', 'forgotPassword', 'resetPassword']
let isAuthenticationPath = computed(() => {
  return authenticationRoutes.includes(router.currentRoute.value.name as string)
});

onMounted(async () => {
  if (!userStore.user.email)
    userStore.setUser(await userService.getCurrentUser())
});

onMounted(() => {
  watchEffect(() => {
    const roles = userStore.user.roles ?? [];
    const body = document.body;
    body.classList.remove("theme-admin", "theme-member");
    if (roles.includes(Role.Admin)) {
      body.classList.add("theme-admin");
      return;
    }
    if (roles.includes(Role.Member)) {
      body.classList.add("theme-member");
    }
  });
});
</script>
