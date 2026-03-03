<template>
  <div class="max-w-lg mx-auto">
    <div v-if="loading" class="flex justify-center py-12">
      <Loader2 class="w-6 h-6 animate-spin text-gray-400" />
    </div>

    <template v-else>
      <!-- ====== VIEW ====== -->
      <div v-if="!editing" class="bg-white rounded-xl border border-gray-200">
        <div class="flex flex-col items-center pt-8 pb-5">
          <div class="w-16 h-16 rounded-full bg-brand-600 text-white flex items-center justify-center text-2xl font-bold">
            {{ initials }}
          </div>
          <h2 class="mt-3 text-lg font-semibold text-gray-900">{{ person.firstName }} {{ person.lastName }}</h2>
          <span class="text-sm text-gray-500">{{ userStore.user.email }}</span>
        </div>

        <div class="border-t border-gray-100 px-6 py-4 space-y-3">
          <div class="flex justify-between text-sm">
            <span class="text-gray-500">{{ $t('global.firstName') }}</span>
            <span class="text-gray-900 font-medium">{{ person.firstName || '—' }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-gray-500">{{ $t('global.lastName') }}</span>
            <span class="text-gray-900 font-medium">{{ person.lastName || '—' }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-gray-500 flex items-center gap-1">{{ $t('global.email') }} <Lock class="w-3 h-3 text-gray-300" /></span>
            <span class="text-gray-900 font-medium">{{ userStore.user.email || '—' }}</span>
          </div>
          <template v-if="isMember">
            <div class="flex justify-between text-sm">
              <span class="text-gray-500">{{ $t('global.phoneNumber') }}</span>
              <span class="text-gray-900 font-medium">{{ person.phoneNumber || '—' }}</span>
            </div>
            <div v-if="person.phoneExtension" class="flex justify-between text-sm">
              <span class="text-gray-500">{{ $t('global.phoneExtension') }}</span>
              <span class="text-gray-900 font-medium">{{ person.phoneExtension }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-500">{{ $t('global.street') }}</span>
              <span class="text-gray-900 font-medium">{{ person.street || '—' }}</span>
            </div>
            <div v-if="person.apartment" class="flex justify-between text-sm">
              <span class="text-gray-500">{{ $t('global.apartment') }}</span>
              <span class="text-gray-900 font-medium">{{ person.apartment }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-500">{{ $t('global.city') }}</span>
              <span class="text-gray-900 font-medium">{{ person.city || '—' }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-500">{{ $t('global.zipCode') }}</span>
              <span class="text-gray-900 font-medium">{{ person.zipCode || '—' }}</span>
            </div>
          </template>
        </div>

        <div class="border-t border-gray-100 px-6 py-4">
          <button
            @click="startEditing"
            class="w-full flex items-center justify-center gap-2 px-4 py-2.5 text-sm font-medium text-brand-600 border border-brand-200 bg-brand-50 hover:bg-brand-100 rounded-lg transition"
          >
            <Pencil class="w-4 h-4" />
            {{ $t('pages.account.edit') }}
          </button>
        </div>
      </div>

      <!-- ====== EDIT ====== -->
      <div v-else class="bg-white rounded-xl border border-gray-200">
        <div class="flex items-center gap-3 px-6 py-5 border-b border-gray-100">
          <div class="w-10 h-10 rounded-full bg-brand-600 text-white flex items-center justify-center text-sm font-bold shrink-0">
            {{ initials }}
          </div>
          <h2 class="text-lg font-semibold text-gray-900">{{ $t('pages.account.edit') }}</h2>
        </div>

        <div v-if="apiErrors.length" class="mx-6 mt-4 p-3 bg-brand-50 border border-brand-200 rounded-lg">
          <p v-for="error in apiErrors" :key="error" class="text-sm text-brand-600">{{ error }}</p>
        </div>

        <form @submit.prevent="handleSave" class="p-6 space-y-4">
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.firstName') }} *</label>
              <input
                v-model="form.firstName"
                @blur="validateField('firstName')"
                type="text"
                class="w-full px-3 py-2 border rounded-lg outline-none transition"
                :class="fieldErrors.firstName ? 'border-brand-500 focus:ring-2 focus:ring-brand-500' : 'border-gray-300 focus:ring-2 focus:ring-brand-500 focus:border-brand-500'"
              />
              <p v-if="fieldErrors.firstName" class="text-sm text-brand-500 mt-1">{{ fieldErrors.firstName }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.lastName') }} *</label>
              <input
                v-model="form.lastName"
                @blur="validateField('lastName')"
                type="text"
                class="w-full px-3 py-2 border rounded-lg outline-none transition"
                :class="fieldErrors.lastName ? 'border-brand-500 focus:ring-2 focus:ring-brand-500' : 'border-gray-300 focus:ring-2 focus:ring-brand-500 focus:border-brand-500'"
              />
              <p v-if="fieldErrors.lastName" class="text-sm text-brand-500 mt-1">{{ fieldErrors.lastName }}</p>
            </div>
          </div>

          <div>
            <div class="flex items-center gap-1.5 mb-1">
              <label class="text-sm font-medium text-gray-400">{{ $t('global.email') }}</label>
              <Lock class="w-3 h-3 text-gray-300" />
            </div>
            <div class="px-3 py-2 bg-gray-50 border border-gray-200 rounded-lg text-sm text-gray-400 select-none">
              {{ userStore.user.email }}
            </div>
          </div>

          <template v-if="isMember">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.phoneNumber') }}</label>
                <input
                  v-model="form.phoneNumber"
                  @blur="validateField('phoneNumber')"
                  type="tel"
                  placeholder="555-555-5555"
                  class="w-full px-3 py-2 border rounded-lg outline-none transition"
                  :class="fieldErrors.phoneNumber ? 'border-brand-500 focus:ring-2 focus:ring-brand-500' : 'border-gray-300 focus:ring-2 focus:ring-brand-500 focus:border-brand-500'"
                />
                <p v-if="fieldErrors.phoneNumber" class="text-sm text-brand-500 mt-1">{{ fieldErrors.phoneNumber }}</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.phoneExtension') }}</label>
                <input v-model.number="form.phoneExtension" type="number" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.street') }}</label>
                <input v-model="form.street" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.apartment') }}</label>
                <input v-model.number="form.apartment" type="number" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.city') }}</label>
                <input v-model="form.city" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.zipCode') }}</label>
                <input
                  v-model="form.zipCode"
                  @blur="validateField('zipCode')"
                  type="text"
                  placeholder="A1A 1A1"
                  class="w-full px-3 py-2 border rounded-lg outline-none transition"
                  :class="fieldErrors.zipCode ? 'border-brand-500 focus:ring-2 focus:ring-brand-500' : 'border-gray-300 focus:ring-2 focus:ring-brand-500 focus:border-brand-500'"
                />
                <p v-if="fieldErrors.zipCode" class="text-sm text-brand-500 mt-1">{{ fieldErrors.zipCode }}</p>
              </div>
            </div>
          </template>

          <div class="flex justify-end gap-3 pt-4 border-t border-gray-100">
            <button
              type="button"
              @click="cancelEditing"
              class="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
            >
              {{ $t('global.cancel') }}
            </button>
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
  </div>
</template>

<script lang="ts" setup>
import {ref, reactive, computed, onMounted} from "vue";
import {useI18n} from "vue3-i18n";
import {useNotification} from "@kyvg/vue3-notification";
import {Loader2, Pencil, Lock} from "lucide-vue-next";
import {useUserStore} from "@/stores/userStore";
import {usePersonStore} from "@/stores/personStore";
import {useMemberService, useAdministratorService} from "@/inversify.config";
import {Role} from "@/types/enums";
import {Member} from "@/types/entities";
import type {IPerson} from "@/types/entities/person";
import {validate} from "@/validation";
import {required, mustMatchPhoneNumberFormat, mustMatchZipCodeFormat} from "@/validation/rules";

const {t} = useI18n();
const {notify} = useNotification();
const userStore = useUserStore();
const personStore = usePersonStore();
const memberService = useMemberService();
const adminService = useAdministratorService();

const loading = ref(true);
const editing = ref(false);
const submitting = ref(false);
const person = ref<IPerson>({});
const apiErrors = ref<string[]>([]);
const fieldErrors = reactive<Record<string, string | undefined>>({});

const isMember = computed(() => userStore.hasRole(Role.Member));
const form = reactive<Partial<Member>>({});

const initials = computed(() => {
  const first = person.value.firstName || "";
  const last = person.value.lastName || "";
  return ((first[0] || "") + (last[0] || "")).toUpperCase();
});

function startEditing() {
  Object.assign(form, {
    id: person.value.id || userStore.user.id,
    firstName: person.value.firstName,
    lastName: person.value.lastName,
    email: (person.value as Member).email || userStore.user.email,
    phoneNumber: (person.value as Member).phoneNumber,
    phoneExtension: (person.value as Member).phoneExtension,
    apartment: (person.value as Member).apartment,
    street: (person.value as Member).street,
    city: (person.value as Member).city,
    zipCode: (person.value as Member).zipCode,
  });
  editing.value = true;
}

function cancelEditing() {
  editing.value = false;
  Object.keys(fieldErrors).forEach(k => fieldErrors[k] = undefined);
  apiErrors.value = [];
}

function validateField(field: string) {
  fieldErrors[field] = undefined;

  if (field === "firstName") {
    const r = validate(form.firstName || "", [required]);
    if (!r.valid) fieldErrors[field] = r.message;
  } else if (field === "lastName") {
    const r = validate(form.lastName || "", [required]);
    if (!r.valid) fieldErrors[field] = r.message;
  } else if (field === "phoneNumber" && form.phoneNumber) {
    const r = validate(form.phoneNumber, [mustMatchPhoneNumberFormat]);
    if (!r.valid) fieldErrors[field] = r.message;
  } else if (field === "zipCode" && form.zipCode) {
    const r = validate(form.zipCode, [mustMatchZipCodeFormat]);
    if (!r.valid) fieldErrors[field] = r.message;
  }
}

function validateAll(): boolean {
  let valid = true;
  Object.keys(fieldErrors).forEach(k => fieldErrors[k] = undefined);

  const fields = ["firstName", "lastName"];
  if (isMember.value) {
    fields.push("phoneNumber", "zipCode");
  }

  for (const field of fields) {
    validateField(field);
    if (fieldErrors[field]) valid = false;
  }
  return valid;
}

async function handleSave() {
  if (!validateAll()) {
    notify({type: "error", text: t("global.formErrorNotification")});
    return;
  }

  submitting.value = true;
  apiErrors.value = [];

  try {
    const response = await memberService.updateMember(form as Member);
    if (response.succeeded) {
      notify({type: "success", text: t("pages.account.updateSuccess")});
      try {
        const updated = isMember.value
          ? await memberService.getAuthenticated()
          : await adminService.getAuthenticated();
        if (updated) {
          person.value = updated;
          personStore.setPerson(updated);
        }
      } catch {
        Object.assign(person.value, form);
        personStore.setPerson(person.value);
      }
      editing.value = false;
    } else {
      apiErrors.value = response.getErrorMessages("pages.members.update.validation");
    }
  } catch {
    notify({type: "error", text: t("pages.account.updateError")});
  }
  submitting.value = false;
}

onMounted(async () => {
  try {
    if (userStore.hasRole(Role.Admin)) {
      const admin = await adminService.getAuthenticated();
      if (admin) {
        person.value = admin;
        personStore.setPerson(admin);
      }
    } else if (userStore.hasRole(Role.Member)) {
      const member = await memberService.getAuthenticated();
      if (member) {
        person.value = member;
        personStore.setPerson(member);
      }
    }
  } catch {
    if (personStore.person?.firstName) {
      person.value = personStore.person;
    }
  }
  loading.value = false;
});
</script>
