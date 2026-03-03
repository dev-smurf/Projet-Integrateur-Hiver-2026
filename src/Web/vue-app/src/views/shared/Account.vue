<template>
  <div class="max-w-2xl">
    <h1 class="text-2xl font-bold text-gray-900 mb-6">{{ $t('routes.account.name') }}</h1>

    <div v-if="loading" class="flex items-center gap-2 text-gray-500">
      <Loader2 class="w-5 h-5 animate-spin" />
    </div>

    <div v-else class="bg-white rounded-xl border border-gray-200 p-6 space-y-4">
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <span class="block text-sm text-gray-500">{{ $t('global.firstName') }}</span>
          <span class="text-gray-900">{{ person.firstName || '—' }}</span>
        </div>
        <div>
          <span class="block text-sm text-gray-500">{{ $t('global.lastName') }}</span>
          <span class="text-gray-900">{{ person.lastName || '—' }}</span>
        </div>
        <div>
          <span class="block text-sm text-gray-500">{{ $t('global.email') }}</span>
          <span class="text-gray-900">{{ userStore.user.email || '—' }}</span>
        </div>
        <div>
          <span class="block text-sm text-gray-500">{{ $t('global.phoneNumber') }}</span>
          <span class="text-gray-900">{{ userStore.user.phoneNumber || '—' }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref, onMounted} from "vue";
import {Loader2} from "lucide-vue-next";
import {useUserStore} from "@/stores/userStore";
import {usePersonStore} from "@/stores/personStore";
import {useMemberService, useAdministratorService} from "@/inversify.config";
import {Role} from "@/types/enums";
import type {IPerson} from "@/types/entities/person";

const userStore = useUserStore();
const personStore = usePersonStore();
const memberService = useMemberService();
const adminService = useAdministratorService();

const loading = ref(true);
const person = ref<IPerson>({});

onMounted(async () => {
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
  loading.value = false;
});
</script>
