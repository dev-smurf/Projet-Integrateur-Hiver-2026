<template>
  <div>
    <h1 class="text-2xl font-bold text-brand-900 mb-1">{{ $t('routes.resetPassword.name') }}</h1>

    <div v-if="successMessage" class="mb-4 p-3 bg-green-50 border border-green-200 rounded-lg">
      <p class="text-sm text-green-700">{{ successMessage }}</p>
    </div>

    <div v-if="errors.length" class="mb-4 p-3 bg-brand-50 border border-brand-200 rounded-lg">
      <p v-for="error in errors" :key="error" class="text-sm text-brand-600">{{ error }}</p>
    </div>

    <form v-if="!successMessage" @submit.prevent="handleResetPassword" class="space-y-4 mt-6">
      <div class="p-4 bg-amber-50 border border-amber-200 rounded-lg">
        <p class="text-sm font-semibold text-amber-900 mb-2">
          {{ $t('pages.resetPassword.passwordRequirements.title') }}
        </p>
        <ul class="space-y-2">
          <li
            v-for="requirement in passwordRequirements"
            :key="requirement.label"
            class="flex items-center gap-2 text-sm"
            :class="requirement.valid ? 'text-emerald-700' : 'text-amber-800'"
          >
            <CheckCircle2 v-if="requirement.valid" class="h-4 w-4 shrink-0" />
            <Circle v-else class="h-4 w-4 shrink-0" />
            <span>{{ requirement.label }}</span>
          </li>
        </ul>
      </div>

      <div>
        <label for="password" class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.password') }}</label>
        <input
          id="password"
          v-model="password"
          type="password"
          autocomplete="new-password"
          class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
          required
        />
      </div>
      <div>
        <label for="passwordConfirmation" class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.passwordConfirmation') }}</label>
        <input
          id="passwordConfirmation"
          v-model="passwordConfirmation"
          type="password"
          autocomplete="new-password"
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
  </div>
</template>

<script lang="ts" setup>
import {computed, ref} from "vue";
import {useRouter} from "vue-router";
import {useI18n} from "vue3-i18n";
import {useAuthenticationService} from "@/inversify.config";
import {CheckCircle2, Circle, Loader2} from "lucide-vue-next";

const props = defineProps<{userId: string; token: string}>();

const router = useRouter();
const {t} = useI18n();
const authService = useAuthenticationService();

const password = ref("");
const passwordConfirmation = ref("");
const loading = ref(false);
const errors = ref<string[]>([]);
const successMessage = ref("");

const passwordRequirements = computed(() => [
  {
    label: t("pages.resetPassword.passwordRequirements.minLength"),
    valid: password.value.length >= 10
  },
  {
    label: t("pages.resetPassword.passwordRequirements.uppercase"),
    valid: /[A-Z]/.test(password.value)
  },
  {
    label: t("pages.resetPassword.passwordRequirements.lowercase"),
    valid: /[a-z]/.test(password.value)
  },
  {
    label: t("pages.resetPassword.passwordRequirements.digit"),
    valid: /\d/.test(password.value)
  },
  {
    label: t("pages.resetPassword.passwordRequirements.special"),
    valid: /[^A-Za-z0-9]/.test(password.value)
  },
  {
    label: t("pages.resetPassword.passwordRequirements.uniqueChars"),
    valid: new Set(password.value).size >= 6
  },
  {
    label: t("pages.resetPassword.passwordRequirements.match"),
    valid: passwordConfirmation.value.length > 0 && password.value === passwordConfirmation.value
  }
]);

async function handleResetPassword() {
  loading.value = true;
  errors.value = [];

  const response = await authService.resetPassword({
    userId: props.userId,
    token: props.token,
    password: password.value,
    passwordConfirmation: passwordConfirmation.value
  });

  if (response.succeeded) {
    successMessage.value = t("pages.resetPassword.validation.success");
    setTimeout(() => router.push({name: "login"}), 3000);
  } else {
    errors.value = response.getErrorMessages("pages.resetPassword.validation");
  }

  loading.value = false;
}
</script>
