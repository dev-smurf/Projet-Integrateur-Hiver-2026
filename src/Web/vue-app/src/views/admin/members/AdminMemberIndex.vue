<template>
  <div>
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900">{{ $t('routes.admin.children.members.name') }}</h1>
      <router-link
        :to="{ name: 'admin.children.members.add' }"
        class="flex items-center gap-2 bg-brand-600 hover:bg-brand-700 text-white font-medium py-2 px-4 rounded-lg transition text-sm"
      >
        <Plus class="w-4 h-4" />
        {{ $t('global.add') }}
      </router-link>
    </div>

    <div class="mb-4">
      <div class="relative">
        <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
        <input
          v-model="searchValue"
          @input="onSearch"
          type="text"
          :placeholder="$t('global.search')"
          class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
        />
      </div>
    </div>

    <div class="bg-white rounded-xl border border-gray-200 overflow-hidden">
      <table class="w-full">
        <thead>
          <tr class="border-b border-gray-200 bg-gray-50">
            <th class="text-left text-xs font-medium text-gray-500 uppercase tracking-wider px-4 py-3">{{ $t('global.firstName') }}</th>
            <th class="text-left text-xs font-medium text-gray-500 uppercase tracking-wider px-4 py-3">{{ $t('global.lastName') }}</th>
            <th class="text-left text-xs font-medium text-gray-500 uppercase tracking-wider px-4 py-3 hidden md:table-cell">{{ $t('global.email') }}</th>
            <th class="text-left text-xs font-medium text-gray-500 uppercase tracking-wider px-4 py-3 hidden lg:table-cell">{{ $t('global.city') }}</th>
            <th class="text-right text-xs font-medium text-gray-500 uppercase tracking-wider px-4 py-3">{{ $t('global.table.actions') }}</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100">
          <tr v-if="loading">
            <td colspan="5" class="px-4 py-8 text-center text-gray-500">
              <Loader2 class="w-5 h-5 animate-spin mx-auto" />
            </td>
          </tr>
          <tr v-else-if="!members.length">
            <td colspan="5" class="px-4 py-8 text-center text-gray-500">{{ $t('global.table.noData') }}</td>
          </tr>
          <tr v-for="member in members" :key="member.id" class="hover:bg-gray-50 transition">
            <td class="px-4 py-3 text-sm text-gray-900">{{ member.firstName }}</td>
            <td class="px-4 py-3 text-sm text-gray-900">{{ member.lastName }}</td>
            <td class="px-4 py-3 text-sm text-gray-600 hidden md:table-cell">{{ member.email }}</td>
            <td class="px-4 py-3 text-sm text-gray-600 hidden lg:table-cell">{{ member.city || '—' }}</td>
            <td class="px-4 py-3 text-right">
              <div class="flex items-center justify-end gap-2">
                <router-link
                  :to="{ name: 'admin.children.members.edit', params: { id: member.id } }"
                  class="p-1.5 text-gray-400 hover:text-brand-600 transition"
                >
                  <Pencil class="w-4 h-4" />
                </router-link>
                <button
                  @click="confirmDelete(member)"
                  class="p-1.5 text-gray-400 hover:text-brand-600 transition"
                >
                  <Trash2 class="w-4 h-4" />
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="totalItems > pageSize" class="flex items-center justify-between mt-4">
      <span class="text-sm text-gray-500">
        {{ (pageIndex - 1) * pageSize + 1 }}–{{ Math.min(pageIndex * pageSize, totalItems) }} {{ $t('global.table.of') }} {{ totalItems }}
      </span>
      <div class="flex gap-2">
        <button
          @click="pageIndex > 1 && changePage(pageIndex - 1)"
          :disabled="pageIndex <= 1"
          class="px-3 py-1.5 text-sm border border-gray-300 rounded-lg hover:bg-gray-50 transition disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <ChevronLeft class="w-4 h-4" />
        </button>
        <button
          @click="pageIndex * pageSize < totalItems && changePage(pageIndex + 1)"
          :disabled="pageIndex * pageSize >= totalItems"
          class="px-3 py-1.5 text-sm border border-gray-300 rounded-lg hover:bg-gray-50 transition disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <ChevronRight class="w-4 h-4" />
        </button>
      </div>
    </div>

    <!-- Delete confirmation modal -->
    <div v-if="memberToDelete" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div class="bg-white rounded-xl p-6 w-full max-w-sm shadow-lg">
        <h3 class="text-lg font-semibold text-gray-900 mb-2">{{ $t('global.delete') }}</h3>
        <p class="text-sm text-gray-600 mb-6">{{ $t('pages.members.delete.confirmation') }}</p>
        <div class="flex justify-end gap-3">
          <button
            @click="memberToDelete = null"
            class="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
          >
            {{ $t('global.cancel') }}
          </button>
          <button
            @click="deleteMember"
            class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition"
          >
            {{ $t('global.delete') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref, computed, onMounted} from "vue";
import {useI18n} from "vue3-i18n";
import {useNotification} from "@kyvg/vue3-notification";
import {Plus, Search, Pencil, Trash2, Loader2, ChevronLeft, ChevronRight} from "lucide-vue-next";
import {useMemberService} from "@/inversify.config";
import type {Member} from "@/types/entities";

const {t} = useI18n();
const {notify} = useNotification();
const memberService = useMemberService();

const allMembers = ref<Member[]>([]);
const loading = ref(true);
const searchValue = ref("");
const pageIndex = ref(1);
const pageSize = 10;
const memberToDelete = ref<Member | null>(null);

const filtered = computed(() => {
  const q = searchValue.value.toLowerCase().trim();
  if (!q) return allMembers.value;
  return allMembers.value.filter(m =>
    (m.firstName || "").toLowerCase().includes(q) ||
    (m.lastName || "").toLowerCase().includes(q) ||
    (m.email || "").toLowerCase().includes(q) ||
    (m.city || "").toLowerCase().includes(q)
  );
});

const totalItems = computed(() => filtered.value.length);

const members = computed(() => {
  const start = (pageIndex.value - 1) * pageSize;
  return filtered.value.slice(start, start + pageSize);
});

// Reset to page 1 when search changes
function onSearch() {
  pageIndex.value = 1;
}

async function fetchMembers() {
  loading.value = true;
  const result = await memberService.search(1, 9999, "");
  allMembers.value = result.items || [];
  loading.value = false;
}

function changePage(page: number) {
  pageIndex.value = page;
}

function confirmDelete(member: Member) {
  memberToDelete.value = member;
}

async function deleteMember() {
  if (!memberToDelete.value?.id) return;
  const response = await memberService.deleteMember(memberToDelete.value.id);
  if (response.succeeded) {
    notify({type: "success", text: t("pages.members.delete.validation.successMessage")});
    memberToDelete.value = null;
    await fetchMembers();
  } else {
    notify({type: "error", text: t("pages.members.delete.validation.failedMessage")});
  }
}

onMounted(fetchMembers);
</script>
