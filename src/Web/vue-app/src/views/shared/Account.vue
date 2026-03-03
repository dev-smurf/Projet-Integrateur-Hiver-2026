<template>
  <div class="max-w-lg mx-auto">
    <div v-if="loading" class="flex justify-center py-12">
      <Loader2 class="w-6 h-6 animate-spin text-gray-400" />
    </div>

    <div v-else class="bg-white rounded-xl border border-gray-200">
      <!-- Avatar + name header -->
      <div class="flex flex-col items-center pt-8 pb-5 relative">
        <div class="w-16 h-16 rounded-full bg-brand-600 text-white flex items-center justify-center text-2xl font-bold">
          {{ initials }}
        </div>
        <h2 class="mt-3 text-lg font-semibold text-gray-900">{{ person.firstName }} {{ person.lastName }}</h2>
        <span class="text-sm text-gray-500">{{ userStore.user.email }}</span>
        <span class="mt-2 inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-brand-50 text-brand-600">
          {{ isMember ? 'Member' : $t('user.roles.admin') }}
        </span>

        <!-- Edit / Cancel button -->
        <button
          v-if="!editing"
          @click="startEditing"
          class="absolute top-4 right-4 p-2 rounded-lg text-gray-400 hover:text-brand-600 hover:bg-brand-50 transition-colors"
          :title="$t('pages.account.edit')"
        >
          <Pencil class="w-4 h-4" />
        </button>
        <button
          v-else
          @click="cancelEditing"
          class="absolute top-4 right-4 p-2 rounded-lg text-gray-400 hover:text-red-600 hover:bg-red-50 transition-colors"
          :title="$t('global.cancel')"
        >
          <X class="w-4 h-4" />
        </button>
      </div>

      <!-- VIEW MODE -->
      <div v-if="!editing" class="border-t border-gray-100 px-6 py-4 space-y-3">
        <div class="flex justify-between text-sm">
          <span class="text-gray-500">{{ $t('global.firstName') }}</span>
          <span class="text-gray-900 font-medium">{{ person.firstName || '—' }}</span>
        </div>
        <div class="flex justify-between text-sm">
          <span class="text-gray-500">{{ $t('global.lastName') }}</span>
          <span class="text-gray-900 font-medium">{{ person.lastName || '—' }}</span>
        </div>
        <div class="flex justify-between text-sm">
          <span class="text-gray-500">{{ $t('global.email') }}</span>
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

      <!-- EDIT MODE -->
      <form v-else @submit.prevent="save" class="border-t border-gray-100 px-6 py-5 space-y-4">
        <!-- Email (read-only) -->
        <div>
          <label class="block text-xs font-medium text-gray-500 mb-1">
            {{ $t('global.email') }}
            <Lock class="inline w-3 h-3 ml-1 text-gray-400" />
          </label>
          <input
            type="text"
            :value="userStore.user.email"
            disabled
            class="w-full px-3 py-2 rounded-lg border border-gray-200 bg-gray-50 text-gray-400 text-sm cursor-not-allowed"
          />
          <p class="text-xs text-gray-400 mt-1">{{ $t('pages.account.readOnly') }}</p>
        </div>

        <!-- First name -->
        <div>
          <label class="block text-xs font-medium text-gray-500 mb-1">
            {{ $t('global.firstName') }} <span class="text-red-500">*</span>
          </label>
          <input
            v-model="form.firstName"
            type="text"
            @blur="touchField('firstName')"
            class="w-full px-3 py-2 rounded-lg border text-sm transition-colors"
            :class="fieldError('firstName') ? 'border-red-400 focus:ring-red-400' : 'border-gray-200 focus:ring-brand-500'"
          />
          <p v-if="fieldError('firstName')" class="text-xs text-red-500 mt-1">{{ fieldError('firstName') }}</p>
        </div>

        <!-- Last name -->
        <div>
          <label class="block text-xs font-medium text-gray-500 mb-1">
            {{ $t('global.lastName') }} <span class="text-red-500">*</span>
          </label>
          <input
            v-model="form.lastName"
            type="text"
            @blur="touchField('lastName')"
            class="w-full px-3 py-2 rounded-lg border text-sm transition-colors"
            :class="fieldError('lastName') ? 'border-red-400 focus:ring-red-400' : 'border-gray-200 focus:ring-brand-500'"
          />
          <p v-if="fieldError('lastName')" class="text-xs text-red-500 mt-1">{{ fieldError('lastName') }}</p>
        </div>

        <!-- Member-only fields -->
        <template v-if="isMember">
          <!-- Phone number -->
          <div>
            <label class="block text-xs font-medium text-gray-500 mb-1">{{ $t('global.phoneNumber') }}</label>
            <input
              v-model="form.phoneNumber"
              type="text"
              placeholder="555-555-5555"
              @blur="touchField('phoneNumber')"
              class="w-full px-3 py-2 rounded-lg border text-sm transition-colors"
              :class="fieldError('phoneNumber') ? 'border-red-400 focus:ring-red-400' : 'border-gray-200 focus:ring-brand-500'"
            />
            <p v-if="fieldError('phoneNumber')" class="text-xs text-red-500 mt-1">{{ fieldError('phoneNumber') }}</p>
          </div>

          <!-- Phone extension -->
          <div>
            <label class="block text-xs font-medium text-gray-500 mb-1">{{ $t('global.phoneExtension') }}</label>
            <input
              v-model.number="form.phoneExtension"
              type="number"
              class="w-full px-3 py-2 rounded-lg border border-gray-200 focus:ring-brand-500 text-sm transition-colors"
            />
          </div>

          <!-- Street -->
          <div>
            <label class="block text-xs font-medium text-gray-500 mb-1">{{ $t('global.street') }}</label>
            <input
              v-model="form.street"
              type="text"
              class="w-full px-3 py-2 rounded-lg border border-gray-200 focus:ring-brand-500 text-sm transition-colors"
            />
          </div>

          <!-- Apartment -->
          <div>
            <label class="block text-xs font-medium text-gray-500 mb-1">{{ $t('global.apartment') }}</label>
            <input
              v-model.number="form.apartment"
              type="number"
              class="w-full px-3 py-2 rounded-lg border border-gray-200 focus:ring-brand-500 text-sm transition-colors"
            />
          </div>

          <!-- City -->
          <div>
            <label class="block text-xs font-medium text-gray-500 mb-1">{{ $t('global.city') }}</label>
            <input
              v-model="form.city"
              type="text"
              class="w-full px-3 py-2 rounded-lg border border-gray-200 focus:ring-brand-500 text-sm transition-colors"
            />
          </div>

          <!-- Zip code -->
          <div>
            <label class="block text-xs font-medium text-gray-500 mb-1">{{ $t('global.zipCode') }}</label>
            <input
              v-model="form.zipCode"
              type="text"
              placeholder="H0H 0H0"
              @blur="touchField('zipCode')"
              class="w-full px-3 py-2 rounded-lg border text-sm transition-colors"
              :class="fieldError('zipCode') ? 'border-red-400 focus:ring-red-400' : 'border-gray-200 focus:ring-brand-500'"
            />
            <p v-if="fieldError('zipCode')" class="text-xs text-red-500 mt-1">{{ fieldError('zipCode') }}</p>
          </div>
        </template>

        <!-- Save button -->
        <button
          type="submit"
          :disabled="submitting"
          class="w-full mt-2 flex items-center justify-center gap-2 px-4 py-2.5 rounded-lg bg-brand-600 text-white text-sm font-medium hover:bg-brand-700 disabled:opacity-50 transition-colors"
        >
          <Loader2 v-if="submitting" class="w-4 h-4 animate-spin" />
          {{ $t('global.save') }}
        </button>
      </form>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref, computed, reactive, onMounted} from "vue";
import {Loader2, Pencil, X, Lock} from "lucide-vue-next";
import {useI18n} from "vue3-i18n";
import {useNotification} from "@kyvg/vue3-notification";
import {useUserStore} from "@/stores/userStore";
import {usePersonStore} from "@/stores/personStore";
import {useMemberService, useAdministratorService} from "@/inversify.config";
import {Role} from "@/types/enums";
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
const person = ref<any>({});
const touched = reactive<Record<string, boolean>>({});

const form = reactive({
  firstName: '',
  lastName: '',
  phoneNumber: '',
  phoneExtension: undefined as number | undefined,
  apartment: undefined as number | undefined,
  street: '',
  city: '',
  zipCode: '',
});

const isMember = computed(() => userStore.hasRole(Role.Member));

const initials = computed(() => {
  const first = person.value.firstName || "";
  const last = person.value.lastName || "";
  return ((first[0] || "") + (last[0] || "")).toUpperCase();
});

function touchField(field: string) {
  touched[field] = true;
}

function fieldError(field: string): string | undefined {
  if (!touched[field]) return undefined;

  if (field === 'firstName') {
    const result = validate(form.firstName, [required]);
    return result.valid ? undefined : result.message;
  }
  if (field === 'lastName') {
    const result = validate(form.lastName, [required]);
    return result.valid ? undefined : result.message;
  }
  if (field === 'phoneNumber' && form.phoneNumber) {
    const result = validate(form.phoneNumber, [mustMatchPhoneNumberFormat]);
    return result.valid ? undefined : result.message;
  }
  if (field === 'zipCode' && form.zipCode) {
    const result = validate(form.zipCode, [mustMatchZipCodeFormat]);
    return result.valid ? undefined : result.message;
  }
  return undefined;
}

function hasErrors(): boolean {
  // Check required fields
  if (!validate(form.firstName, [required]).valid) return true;
  if (!validate(form.lastName, [required]).valid) return true;
  // Check optional format fields for members
  if (isMember.value) {
    if (form.phoneNumber && !validate(form.phoneNumber, [mustMatchPhoneNumberFormat]).valid) return true;
    if (form.zipCode && !validate(form.zipCode, [mustMatchZipCodeFormat]).valid) return true;
  }
  return false;
}

function startEditing() {
  form.firstName = person.value.firstName || '';
  form.lastName = person.value.lastName || '';
  form.phoneNumber = person.value.phoneNumber || '';
  form.phoneExtension = person.value.phoneExtension || undefined;
  form.apartment = person.value.apartment || undefined;
  form.street = person.value.street || '';
  form.city = person.value.city || '';
  form.zipCode = person.value.zipCode || '';
  Object.keys(touched).forEach(k => touched[k] = false);
  editing.value = true;
}

function cancelEditing() {
  editing.value = false;
  Object.keys(touched).forEach(k => touched[k] = false);
}

async function save() {
  // Touch all required fields to show errors
  touched.firstName = true;
  touched.lastName = true;
  if (isMember.value) {
    touched.phoneNumber = true;
    touched.zipCode = true;
  }

  if (hasErrors()) {
    notify({type: "error", text: t("global.formErrorNotification")});
    return;
  }

  submitting.value = true;
  try {
    let result;
    if (isMember.value) {
      result = await memberService.updateMyProfile({
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        phoneNumber: form.phoneNumber?.trim() || undefined,
        phoneExtension: form.phoneExtension || undefined,
        apartment: form.apartment || undefined,
        street: form.street?.trim() || undefined,
        city: form.city?.trim() || undefined,
        zipCode: form.zipCode?.trim() || undefined,
      });
    } else {
      result = await adminService.updateMyProfile({
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
      });
    }

    if (result.succeeded) {
      // Update local state
      person.value.firstName = form.firstName.trim();
      person.value.lastName = form.lastName.trim();
      if (isMember.value) {
        person.value.phoneNumber = form.phoneNumber?.trim() || undefined;
        person.value.phoneExtension = form.phoneExtension || undefined;
        person.value.apartment = form.apartment || undefined;
        person.value.street = form.street?.trim() || undefined;
        person.value.city = form.city?.trim() || undefined;
        person.value.zipCode = form.zipCode?.trim() || undefined;
      }
      personStore.setPerson(person.value);
      editing.value = false;
      notify({type: "success", text: t("pages.account.updateSuccess")});
    } else {
      notify({type: "error", text: t("pages.account.updateError")});
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
