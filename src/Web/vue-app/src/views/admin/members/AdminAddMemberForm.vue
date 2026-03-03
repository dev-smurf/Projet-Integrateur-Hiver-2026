<template>
  <div class="max-w-2xl">
    <h1 class="text-2xl font-bold text-gray-900 mb-6">{{ $t('routes.admin.children.members.add.name') }}</h1>

    <div v-if="apiErrors.length" class="mb-4 p-3 bg-brand-50 border border-brand-200 rounded-lg">
      <p v-for="error in apiErrors" :key="error" class="text-sm text-brand-600">{{ error }}</p>
    </div>

    <form @submit.prevent="handleSubmit" class="bg-white rounded-xl border border-gray-200 p-6 space-y-4">
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.firstName') }} *</label>
          <input v-model="member.firstName" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
          <p v-if="fieldErrors.firstName" class="text-sm text-brand-500 mt-1">{{ fieldErrors.firstName }}</p>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.lastName') }} *</label>
          <input v-model="member.lastName" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
          <p v-if="fieldErrors.lastName" class="text-sm text-brand-500 mt-1">{{ fieldErrors.lastName }}</p>
        </div>
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.email') }} *</label>
        <input v-model="member.email" type="email" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
        <p v-if="fieldErrors.email" class="text-sm text-brand-500 mt-1">{{ fieldErrors.email }}</p>
      </div>
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.phoneNumber') }}</label>
          <input v-model="member.phoneNumber" type="tel" placeholder="555-555-5555" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
          <p v-if="fieldErrors.phoneNumber" class="text-sm text-brand-500 mt-1">{{ fieldErrors.phoneNumber }}</p>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.phoneExtension') }}</label>
          <input v-model.number="member.phoneExtension" type="number" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
        </div>
      </div>
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.apartment') }}</label>
          <input v-model.number="member.apartment" type="number" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.street') }}</label>
          <input v-model="member.street" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
        </div>
      </div>
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.city') }}</label>
          <input v-model="member.city" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.zipCode') }}</label>
          <input v-model="member.zipCode" type="text" placeholder="A1A 1A1" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
          <p v-if="fieldErrors.zipCode" class="text-sm text-brand-500 mt-1">{{ fieldErrors.zipCode }}</p>
        </div>
      </div>

      <div class="flex justify-end gap-3 pt-4 border-t border-gray-100">
        <router-link
          :to="{ name: 'admin.children.members.index' }"
          class="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
        >
          {{ $t('global.cancel') }}
        </router-link>
        <button
          type="submit"
          :disabled="submitting"
          class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <Loader2 v-if="submitting" class="w-4 h-4 animate-spin" />
          {{ $t('global.save') }}
        </button>
      </div>
    </form>
  </div>
</template>

<script lang="ts" setup>
import {ref, reactive} from "vue";
import {useRouter} from "vue-router";
import {useI18n} from "vue3-i18n";
import {useNotification} from "@kyvg/vue3-notification";
import {Loader2} from "lucide-vue-next";
import {useMemberService} from "@/inversify.config";
import {Member} from "@/types/entities";
import {validate} from "@/validation";
import {required, mustMatchEmailFormat, mustMatchPhoneNumberFormat, mustMatchZipCodeFormat} from "@/validation/rules";

const router = useRouter();
const {t} = useI18n();
const {notify} = useNotification();
const memberService = useMemberService();

const member = reactive(new Member());
const submitting = ref(false);
const apiErrors = ref<string[]>([]);
const fieldErrors = reactive<Record<string, string | undefined>>({});

function validateForm(): boolean {
  let valid = true;
  Object.keys(fieldErrors).forEach(k => fieldErrors[k] = undefined);

  const checks: [string, ReturnType<typeof validate>][] = [
    ["firstName", validate(member.firstName || "", [required])],
    ["lastName", validate(member.lastName || "", [required])],
    ["email", validate(member.email || "", [required, mustMatchEmailFormat])],
  ];

  if (member.phoneNumber) checks.push(["phoneNumber", validate(member.phoneNumber, [mustMatchPhoneNumberFormat])]);
  if (member.zipCode) checks.push(["zipCode", validate(member.zipCode, [mustMatchZipCodeFormat])]);

  for (const [field, result] of checks) {
    if (!result.valid) {
      fieldErrors[field] = result.message;
      valid = false;
    }
  }
  return valid;
}

async function handleSubmit() {
  if (!validateForm()) {
    notify({type: "error", text: t("global.formErrorNotification")});
    return;
  }

  submitting.value = true;
  apiErrors.value = [];

  const response = await memberService.createMember(member);
  if (response.succeeded) {
    notify({type: "success", text: t("pages.members.create.validation.successMessage")});
    await router.push({name: "admin.children.members.index"});
  } else {
    apiErrors.value = response.getErrorMessages("pages.members.create.validation");
  }
  submitting.value = false;
}
</script>
