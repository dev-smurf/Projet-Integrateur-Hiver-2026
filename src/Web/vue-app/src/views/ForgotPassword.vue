<template>
  <div>
    <h1 class="text-2xl font-bold text-brand-900 mb-1">{{ $t('routes.forgotPassword.name') }}</h1>
    <p class="text-gray-500 text-sm mb-6">{{ $t('pages.forgotPassword.tooltip') }}</p>

    <div v-if="successMessage" class="mb-4 p-3 bg-green-50 border border-green-200 rounded-lg">
      <p class="text-sm text-green-700">{{ successMessage }}</p>
    </div>

    <div
      v-if="errors.length"
      class="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg"
      aria-live="polite"
    >
      <p v-for="error in errors" :key="error" class="text-sm font-medium text-red-700">{{ error }}</p>
    </div>

    <form @submit.prevent="handleForgotPassword" class="space-y-4">
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
      <button
        type="submit"
        :disabled="loading"
        class="w-full bg-brand-600 hover:bg-brand-700 text-white font-medium py-2 px-4 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed"
      >
        <span v-if="loading"><Loader2 class="w-4 h-4 animate-spin mx-auto" /></span>
        <span v-else>{{ $t('global.submit') }}</span>
      </button>
    </form>

    <router-link :to="{ name: 'login' }" class="block text-center text-sm text-brand-600 hover:text-brand-700 mt-4">
      {{ $t('global.back') }}
    </router-link>
  </div>
</template>

<script lang="ts" setup>
import {ref} from "vue";
import {useI18n} from "vue3-i18n";
import {useAuthenticationService} from "@/inversify.config";
import {Loader2} from "lucide-vue-next";

const {t} = useI18n();
const authService = useAuthenticationService();

const username = ref("");
const loading = ref(false);
const errors = ref<string[]>([]);
const successMessage = ref("");

async function handleForgotPassword() {
  loading.value = true;
  errors.value = [];
  successMessage.value = "";

  const response = await authService.forgotPassword({
    username: username.value,
    resetPasswordRelativeUrl: t("routes.resetPassword.fullPath")
  });

  if (response.succeeded) {
    successMessage.value = t("pages.forgotPassword.validation.success");
  } else {
    errors.value = response.getErrorMessages("pages.forgotPassword.validation");
  }

  loading.value = false;
}
</script>
