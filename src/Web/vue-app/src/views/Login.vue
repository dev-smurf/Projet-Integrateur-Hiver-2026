<template>
  <div>
    <h1 class="text-2xl font-bold text-brand-900 mb-1">{{ $t('routes.login.name') }}</h1>
    <p class="text-gray-500 text-sm mb-6">{{ $t('pages.dashboard.welcome') }}</p>

    <div v-if="errors.length" class="mb-4 p-3 bg-brand-50 border border-brand-200 rounded-lg">
      <p v-for="error in errors" :key="error" class="text-sm text-brand-600">{{ error }}</p>
    </div>

    <form @submit.prevent="handleLogin" class="space-y-4">
      <div>
        <label for="username" class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.username') }}</label>
        <input
          id="username"
          v-model="username"
          type="email"
          autocomplete="username"
          class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
          required
        />
      </div>
      <div>
        <label for="password" class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.password') }}</label>
        <input
          id="password"
          v-model="password"
          type="password"
          autocomplete="current-password"
          class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
          required
        />
      </div>
      <button
        type="submit"
        :disabled="loading"
        class="w-full bg-brand-600 hover:bg-brand-500 text-white font-medium py-2 px-4 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed"
      >
        <span v-if="loading" class="flex items-center justify-center gap-2">
          <Loader2 class="w-4 h-4 animate-spin" />
        </span>
        <span v-else>{{ $t('pages.login.submit') }}</span>
      </button>
    </form>

    <router-link :to="{ name: 'forgotPassword' }" class="block text-center text-sm text-brand-600 hover:text-brand-500 mt-4">
      {{ $t('pages.login.forgotPassword') }}
    </router-link>
  </div>
</template>

<script lang="ts" setup>
import {ref} from "vue";
import {useRouter} from "vue-router";
import {useAuthenticationService, useUserService} from "@/inversify.config";
import {useUserStore} from "@/stores/userStore";
import {Loader2} from "lucide-vue-next";
import {Role} from "@/types/enums";

const router = useRouter();
const authService = useAuthenticationService();
const userStore = useUserStore();
const userService = useUserService();

const username = ref("");
const password = ref("");
const loading = ref(false);
const errors = ref<string[]>([]);

async function handleLogin() {
  loading.value = true;
  errors.value = [];

  try {
    const response = await authService.login({
      username: username.value,
      password: password.value
    });

    if (response.succeeded) {
      userStore.setUsername(username.value);
      userStore.setUser(await userService.getCurrentUser());
      await router.push({ name: "dashboard" });
    } else {
      const backendErrors = response.getErrorMessages("pages.login.validation");

      if (backendErrors.length) {
        errors.value = backendErrors;
      } else {
        errors.value = ["Identifiant ou mot de passe incorrect"];
      }
    }
  } catch (error) {
    errors.value = ["Impossible de se connecter au serveur. Réessaie plus tard."];
  }

  loading.value = false;
}
</script>
