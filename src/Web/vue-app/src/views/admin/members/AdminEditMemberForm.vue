<template>
  <div class="max-w-2xl mx-auto">
    <h1 class="text-2xl font-bold text-gray-900 mb-6">{{ $t('routes.admin.children.members.edit.name') }}</h1>

    <!-- Skeleton loading -->
    <div v-if="loadingMember" class="bg-white rounded-xl border border-gray-200 p-6 space-y-4 animate-pulse">
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div><div class="h-4 bg-gray-200 rounded w-20 mb-2" /><div class="h-10 bg-gray-200 rounded" /></div>
        <div><div class="h-4 bg-gray-200 rounded w-20 mb-2" /><div class="h-10 bg-gray-200 rounded" /></div>
      </div>
      <div><div class="h-4 bg-gray-200 rounded w-16 mb-2" /><div class="h-10 bg-gray-200 rounded" /></div>
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div><div class="h-4 bg-gray-200 rounded w-24 mb-2" /><div class="h-10 bg-gray-200 rounded" /></div>
        <div><div class="h-4 bg-gray-200 rounded w-24 mb-2" /><div class="h-10 bg-gray-200 rounded" /></div>
      </div>
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div><div class="h-4 bg-gray-200 rounded w-20 mb-2" /><div class="h-10 bg-gray-200 rounded" /></div>
        <div><div class="h-4 bg-gray-200 rounded w-16 mb-2" /><div class="h-10 bg-gray-200 rounded" /></div>
      </div>
      <div class="flex justify-end gap-3 pt-4 border-t border-gray-100">
        <div class="h-9 bg-gray-200 rounded w-20" />
        <div class="h-9 bg-gray-200 rounded w-24" />
      </div>
    </div>

    <template v-else>
      <div v-if="apiErrors.length" class="mb-4 p-3 bg-brand-50 border border-brand-200 rounded-lg">
        <p v-for="error in apiErrors" :key="error" class="text-sm text-brand-600">{{ error }}</p>
      </div>

      <form @submit.prevent="handleSubmit" class="bg-white rounded-xl border border-gray-200 p-6 space-y-4">
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.firstName') }} <span class="text-red-500">*</span></label>
            <input
              v-model="member.firstName"
              type="text"
              @blur="validateField('firstName', member.firstName || '', [required])"
              class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
              :class="fieldErrors.firstName ? 'border-red-400' : 'border-gray-300'"
            />
            <p v-if="fieldErrors.firstName" class="text-sm text-red-500 mt-1">{{ fieldErrors.firstName }}</p>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.lastName') }} <span class="text-red-500">*</span></label>
            <input
              v-model="member.lastName"
              type="text"
              @blur="validateField('lastName', member.lastName || '', [required])"
              class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
              :class="fieldErrors.lastName ? 'border-red-400' : 'border-gray-300'"
            />
            <p v-if="fieldErrors.lastName" class="text-sm text-red-500 mt-1">{{ fieldErrors.lastName }}</p>
          </div>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.email') }} <span class="text-red-500">*</span></label>
          <input
            v-model="member.email"
            type="email"
            @blur="validateField('email', member.email || '', [required, mustMatchEmailFormat])"
            class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
            :class="fieldErrors.email ? 'border-red-400' : 'border-gray-300'"
          />
          <p v-if="fieldErrors.email" class="text-sm text-red-500 mt-1">{{ fieldErrors.email }}</p>
        </div>
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.phoneNumber') }}</label>
            <input
              v-model="member.phoneNumber"
              type="tel"
              placeholder="555-555-5555"
              inputmode="numeric"
              maxlength="12"
              @input="handlePhoneNumberInput"
              @blur="if (member.phoneNumber) validateField('phoneNumber', member.phoneNumber, [mustMatchPhoneNumberFormat]); else fieldErrors.phoneNumber = undefined;"
              class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
              :class="fieldErrors.phoneNumber ? 'border-red-400' : 'border-gray-300'"
            />
            <p v-if="fieldErrors.phoneNumber" class="text-sm text-red-500 mt-1">{{ fieldErrors.phoneNumber }}</p>
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
            <input
              v-model="member.zipCode"
              type="text"
              placeholder="A1A 1A1"
              maxlength="7"
              @input="handleZipCodeInput"
              @blur="if (member.zipCode) validateField('zipCode', member.zipCode, [mustMatchZipCodeFormat]); else fieldErrors.zipCode = undefined;"
              class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
              :class="fieldErrors.zipCode ? 'border-red-400' : 'border-gray-300'"
            />
            <p v-if="fieldErrors.zipCode" class="text-sm text-red-500 mt-1">{{ fieldErrors.zipCode }}</p>
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('pages.memberForm.teams') }}</label>
          <Select2Multi
            v-model="member.equipeIds"
            :options="equipeOptions"
            :placeholder="$t('pages.memberForm.teamsPlaceholder')"
            :search-placeholder="$t('pages.memberForm.teamsSearchPlaceholder')"
            :empty-text="$t('pages.memberForm.teamsEmpty')"
          />
          <p class="mt-1 text-xs text-gray-500">{{ $t('pages.memberForm.teamsOptional') }}</p>
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
    </template>
  </div>
</template>

<script lang="ts" setup>
import {computed, onMounted, reactive, ref} from "vue";
import {useRouter, useRoute} from "vue-router";
import {useI18n} from "vue3-i18n";
import {useNotification} from "@kyvg/vue3-notification";
import {Loader2} from "lucide-vue-next";
import {useEquipesService, useMemberService} from "@/inversify.config";
import Select2Multi from "@/components/forms/Select2Multi.vue";
import {Equipe, Member} from "@/types/entities";
import {validate} from "@/validation";
import {formatPhoneNumberInput, formatPostalCodeInput} from "@/validation/formatters";
import type {Rule} from "@/validation/rules";
import {required, mustMatchEmailFormat, mustMatchPhoneNumberFormat, mustMatchZipCodeFormat} from "@/validation/rules";

const router = useRouter();
const route = useRoute();
const {t} = useI18n();
const {notify} = useNotification();
const memberService = useMemberService();
const equipeService = useEquipesService();

const member = reactive(new Member());
const loadingMember = ref(true);
const submitting = ref(false);
const apiErrors = ref<string[]>([]);
const fieldErrors = reactive<Record<string, string | undefined>>({});
const equipes = ref<Equipe[]>([]);

const equipeOptions = computed(() =>
  equipes.value.map((equipe) => ({
    value: String((equipe as any).id ?? equipe.Id),
    label: String((equipe as any).nameFr ?? (equipe as any).NameFr ?? (equipe as any).nameEn ?? (equipe as any).NameEn ?? ""),
  })).filter((option) => option.value && option.label),
);

onMounted(loadMemberData);

type MemberApiResponse = Partial<Member> & {
  Id?: string;
  UserId?: string;
  Created?: string;
  Active?: boolean;
  AccountActivated?: boolean;
  FirstName?: string;
  LastName?: string;
  FullName?: string;
  Email?: string;
  PhoneNumber?: string;
  PhoneExtension?: number;
  Apartment?: number;
  Street?: string;
  City?: string;
  ZipCode?: string;
  AdminNotes?: string;
  AdminNotesVisibleToMember?: boolean;
  Roles?: string[];
  EquipeIds?: string[];
};

async function loadMemberData() {
  loadingMember.value = true;
  apiErrors.value = [];

  try {
    const memberId = String(route.params.id ?? "");
    const [equipesData, data] = await Promise.all([
      equipeService.getAllEquipes(),
      memberService.getMember(memberId),
    ]);

    const payload = data as MemberApiResponse;
    const resolvedId = payload.id ?? payload.Id;
    if (!resolvedId) {
      throw new Error("Member payload is missing its identifier.");
    }

    equipes.value = equipesData;
    hydrateMember(payload, memberId);
  } catch (error) {
    console.error("Erreur lors du chargement du membre:", error);
    apiErrors.value = ["Impossible de charger les donnees du membre."];
  } finally {
    loadingMember.value = false;
  }
}

function hydrateMember(data: MemberApiResponse, fallbackId: string) {
  member.id = data.id ?? data.Id ?? fallbackId;
  member.userId = data.userId ?? data.UserId;
  member.created = data.created ?? data.Created;
  member.active = data.active ?? data.Active;
  member.accountActivated = data.accountActivated ?? data.AccountActivated;
  member.firstName = data.firstName ?? data.FirstName ?? "";
  member.lastName = data.lastName ?? data.LastName ?? "";
  member.fullName = data.fullName ?? data.FullName ?? "";
  member.email = data.email ?? data.Email ?? "";
  member.phoneNumber = data.phoneNumber ?? data.PhoneNumber ?? "";
  member.phoneExtension = data.phoneExtension ?? data.PhoneExtension;
  member.apartment = data.apartment ?? data.Apartment;
  member.street = data.street ?? data.Street ?? "";
  member.city = data.city ?? data.City ?? "";
  member.zipCode = data.zipCode ?? data.ZipCode ?? "";
  member.adminNotes = data.adminNotes ?? data.AdminNotes;
  member.adminNotesVisibleToMember = data.adminNotesVisibleToMember ?? data.AdminNotesVisibleToMember;
  member.roles = data.roles ?? data.Roles ?? [];
  member.equipeIds = (data.equipeIds ?? data.EquipeIds ?? []).map(String);
}

function validateField(field: string, value: string, rules: Rule[]) {
  const result = validate(value, rules);
  fieldErrors[field] = result.valid ? undefined : result.message;
}

function handlePhoneNumberInput(event: Event) {
  const input = event.target as HTMLInputElement;
  const formattedValue = formatPhoneNumberInput(input.value);
  member.phoneNumber = formattedValue;
  input.value = formattedValue;
}

function handleZipCodeInput(event: Event) {
  const input = event.target as HTMLInputElement;
  const formattedValue = formatPostalCodeInput(input.value);
  member.zipCode = formattedValue;
  input.value = formattedValue;
}

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

  const response = await memberService.updateMember({
    ...member,
    equipeIds: member.equipeIds ?? [],
  });
  if (response.succeeded) {
    notify({type: "success", text: t("pages.members.update.validation.successMessage")});
    await router.push({name: "admin.children.members.index"});
  } else {
    apiErrors.value = response.getErrorMessages("pages.members.update.validation");
  }
  submitting.value = false;
}
</script>
