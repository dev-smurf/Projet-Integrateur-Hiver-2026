<template>
  <template v-if="userStore">
    <AdminAccount v-if="userStore.hasRole(Role.Admin)" />
    <!--<MemberAccount v-else-if="userStore.hasRole(Role.Member)" />-->
    <MemberHome v-if="userStore.hasRole(Role.Member)"/>
  </template>
</template>

<script lang="ts" setup>
import { Role } from "@/types/enums";
    //import MemberAccount from "@/views/member/MemberAccount.vue";
import MemberHome from "@/views/member/MemberHome.vue"
import AdminAccount from "@/views/admin/AdminAccount.vue";
import { useUserStore } from "@/stores/userStore";

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
