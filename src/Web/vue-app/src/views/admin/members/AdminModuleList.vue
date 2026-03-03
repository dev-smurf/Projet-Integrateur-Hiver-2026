<template>
  <div>
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900">{{ $t('routes.admin.children.modules.name') }}</h1>
      <div class="flex items-center gap-4">
        <div class="relative">
          <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input
            v-model="searchValue"
            @input="onSearch"
            type="text"
            :placeholder="$t('global.search')"
            class="pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition text-sm"
          />
        </div>
        <router-link
          :to="{ name: 'admin.children.modules.add' }"
          class="flex items-center gap-2 bg-brand-600 hover:bg-brand-700 text-white font-medium py-2 px-4 rounded-lg transition text-sm"
        >
          <Plus class="w-4 h-4" />
          {{ $t('global.add') }}
        </router-link>
      </div>
    </div>

    <!-- Skeleton loading -->
    <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="n in 6" :key="n" class="bg-white rounded-xl border border-gray-200 overflow-hidden animate-pulse">
        <div class="h-44 bg-gray-200" />
        <div class="p-4 space-y-3">
          <div class="h-5 bg-gray-200 rounded w-3/4" />
          <div class="h-3 bg-gray-200 rounded w-full" />
          <div class="h-3 bg-gray-200 rounded w-2/3" />
        </div>
        <div class="border-t border-gray-100 px-4 py-3 flex justify-around">
          <div class="h-4 bg-gray-200 rounded w-16" />
          <div class="h-4 bg-gray-200 rounded w-20" />
        </div>
      </div>
    </div>

    <!-- Empty state -->
    <div v-else-if="!filtered.length" class="text-center py-12 text-gray-500">
      {{ $t('global.table.noData') }}
    </div>

    <!-- Card grid -->
    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="mod in paginatedModules"
        :key="mod.id"
        class="bg-white rounded-xl border border-gray-200 overflow-hidden flex flex-col hover:shadow-md transition-shadow"
      >
        <!-- Image / Fallback -->
        <div class="h-44 bg-gray-50 flex items-center justify-center overflow-hidden">
          <img
            v-if="mod.cardImageUrl"
            :src="imageUrl(mod.cardImageUrl)"
            :alt="mod.nameFr"
            class="w-full h-full object-cover"
          />
          <BookOpen v-else class="w-12 h-12 text-brand-400" />
        </div>

        <!-- Content -->
        <div class="p-4 flex flex-col flex-1">
          <h3 class="font-semibold text-gray-900 mb-1 line-clamp-1">{{ mod.nameFr || '—' }}</h3>
          <p class="text-sm text-gray-500 line-clamp-3 flex-1">{{ mod.contenueFr || mod.sujetFr || '—' }}</p>
        </div>

        <!-- Actions -->
        <div class="border-t border-gray-100 px-4 py-3 flex items-center justify-around">
          <router-link
            :to="{ name: 'admin.children.modules.edit', params: { id: mod.id } }"
            class="flex items-center gap-1.5 text-sm text-gray-600 hover:text-brand-600 transition"
          >
            <Pencil class="w-4 h-4" />
            {{ $t('global.edit') }}
          </router-link>
          <button
            @click="confirmDelete(mod)"
            class="flex items-center gap-1.5 text-sm text-gray-600 hover:text-brand-600 transition cursor-pointer"
          >
            <Trash2 class="w-4 h-4" />
            {{ $t('global.delete') }}
          </button>
        </div>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="totalItems > pageSize" class="flex items-center justify-between mt-6">
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
    <div v-if="moduleToDelete" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div class="bg-white rounded-xl p-6 w-full max-w-sm shadow-lg">
        <h3 class="text-lg font-semibold text-gray-900 mb-2">{{ $t('global.delete') }}</h3>
        <p class="text-sm text-gray-600 mb-6">{{ moduleToDelete.nameFr }}</p>
        <div class="flex justify-end gap-3">
          <button
            @click="moduleToDelete = null"
            class="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
          >
            {{ $t('global.cancel') }}
          </button>
          <button
            @click="handleDelete"
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
import {useNotification} from "@kyvg/vue3-notification";
import {Plus, Search, Pencil, Trash2, ChevronLeft, ChevronRight, BookOpen} from "lucide-vue-next";
import {useModulesService} from "@/inversify.config";
import type {ModuleDto} from "@/types/entities";

const backendUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/api$/, '');

const {notify} = useNotification();
const modulesService = useModulesService();

const allModules = ref<ModuleDto[]>([]);
const loading = ref(true);
const searchValue = ref("");
const pageIndex = ref(1);
const pageSize = 9;
const moduleToDelete = ref<ModuleDto | null>(null);

function imageUrl(path: string | undefined): string {
  if (!path) return '';
  if (path.startsWith('http')) return path;
  return backendUrl + path;
}

const filtered = computed(() => {
  const q = searchValue.value.toLowerCase().trim();
  if (!q) return allModules.value;
  return allModules.value.filter(m =>
    (m.nameFr || "").toLowerCase().includes(q) ||
    (m.sujetFr || "").toLowerCase().includes(q) ||
    (m.contenueFr || "").toLowerCase().includes(q)
  );
});

const totalItems = computed(() => filtered.value.length);

const paginatedModules = computed(() => {
  const start = (pageIndex.value - 1) * pageSize;
  return filtered.value.slice(start, start + pageSize);
});

function onSearch() {
  pageIndex.value = 1;
}

async function fetchModules() {
  loading.value = true;
  try {
    allModules.value = await modulesService.getAllModules();
  } catch {
    allModules.value = [];
  }
  loading.value = false;
}

function changePage(page: number) {
  pageIndex.value = page;
}

function confirmDelete(mod: ModuleDto) {
  moduleToDelete.value = mod;
}

async function handleDelete() {
  if (!moduleToDelete.value?.id) return;
  try {
    const result = await modulesService.deleteModule(moduleToDelete.value.id);
    if (result.succeeded) {
      notify({type: "success", text: "Module deleted."});
      moduleToDelete.value = null;
      await fetchModules();
    } else {
      notify({type: "error", text: "Error deleting module."});
    }
  } catch {
    notify({type: "error", text: "Error deleting module."});
  }
}

onMounted(fetchModules);
</script>
