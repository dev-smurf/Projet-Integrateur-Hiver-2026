<template>
  <div>
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900">
        {{ $t("routes.admin.children.modules.name") }}
      </h1>
      <div class="flex items-center gap-4">
        <div class="relative">
          <Search
            class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400"
          />
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
          {{ $t("global.add") }}
        </router-link>
      </div>
    </div>

    <!-- Skeleton loading -->
    <div
      v-if="loading"
      class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"
    >
      <div
        v-for="n in 3"
        :key="n"
        class="bg-white rounded-lg shadow-md overflow-hidden animate-pulse"
      >
        <div class="h-40 bg-gray-200" />
        <div class="p-4">
          <div class="h-4 bg-gray-200 rounded w-3/4 mb-3" />
          <div class="h-3 bg-gray-200 rounded w-full mb-2" />
          <div class="h-3 bg-gray-200 rounded w-2/3" />
        </div>
      </div>
    </div>

    <!-- Empty state -->
    <div v-else-if="!filtered.length" class="text-center py-12 text-gray-500">
      {{ $t("global.table.noData") }}
    </div>

    <!-- Card grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
      <div
        v-for="mod in paginatedModules"
        :key="mod.id"
        class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition"
      >
        <!-- Image / Fallback -->
        <div class="h-40 bg-gray-200 relative overflow-hidden group">
          <img
            v-if="mod.cardImageUrl"
            :src="imageUrl(mod.cardImageUrl)"
            :alt="mod.name"
            class="w-full h-full object-cover"
          />
          <div v-else class="w-full h-full bg-gradient-to-br from-brand-400 to-brand-600 flex items-center justify-center">
            <BookOpen class="w-16 h-16 text-white opacity-50" />
          </div>

          <router-link
            :to="{ name: 'admin.children.modules.preview', params: { id: mod.id } }"
            class="absolute inset-0 bg-black/60 opacity-0 group-hover:opacity-100 transition flex items-center justify-center pointer-events-none group-hover:pointer-events-auto"
          >
            <div class="flex flex-col items-center gap-2">
              <Eye class="w-8 h-8 text-white" />
              <span class="text-white font-medium">Apercu</span>
            </div>
          </router-link>
        </div>

        <!-- Content -->
        <div class="p-4">
          <h2 class="text-lg font-bold text-gray-900 mb-2">{{ mod.name || '---' }}</h2>
          <p v-if="mod.subject || mod.content" class="text-sm text-gray-600 mb-3 line-clamp-2">
            {{ mod.subject || plainText(mod.content) }}
          </p>

          <div class="text-xs text-gray-500 mb-4">
            {{ mod.sections?.length || 0 }} {{ $t("Form_Add_Module.fields.sections") }}
          </div>

          <!-- Action Buttons -->
          <div class="flex gap-2 flex-wrap">
            <router-link
              :to="{ name: 'admin.children.modules.edit', params: { id: mod.id } }"
              class="flex-1 min-w-24 flex items-center justify-center gap-1 bg-blue-50 text-blue-600 hover:bg-blue-100 py-2 px-3 rounded text-sm font-medium transition"
            >
              <Pencil class="w-4 h-4" />
              {{ $t("global.update") }}
            </router-link>
            <button
              @click="openAssignModal(mod)"
              class="flex-1 min-w-24 flex items-center justify-center gap-1 bg-green-50 text-green-600 hover:bg-green-100 py-2 px-3 rounded text-sm font-medium transition"
            >
              <Users class="w-4 h-4" />
              Assigner
            </button>
            <button
              @click="confirmDelete(mod)"
              class="flex-1 min-w-24 flex items-center justify-center gap-1 bg-red-50 text-red-600 hover:bg-red-100 py-2 px-3 rounded text-sm font-medium transition"
            >
              <Trash2 class="w-4 h-4" />
              {{ $t("global.delete") }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <AssignModuleModal
      v-if="showAssignModal && selectedModuleForAssignment"
      :module-id="selectedModuleForAssignment.id"
      :module-title="selectedModuleForAssignment.name || 'Module'"
      @close="showAssignModal = false; selectedModuleForAssignment = null"
      @assigned="onAssigned"
    />

    <!-- Pagination -->
    <div
      v-if="totalItems > pageSize"
      class="flex items-center justify-between mt-6"
    >
      <span class="text-sm text-gray-500">
        {{ (pageIndex - 1) * pageSize + 1 }}–{{
          Math.min(pageIndex * pageSize, totalItems)
        }}
        {{ $t("global.table.of") }} {{ totalItems }}
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
          @click="
            pageIndex * pageSize < totalItems && changePage(pageIndex + 1)
          "
          :disabled="pageIndex * pageSize >= totalItems"
          class="px-3 py-1.5 text-sm border border-gray-300 rounded-lg hover:bg-gray-50 transition disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <ChevronRight class="w-4 h-4" />
        </button>
      </div>
    </div>

    <!-- Delete confirmation modal -->
    <div
      v-if="moduleToDelete"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/50"
    >
      <div class="bg-white rounded-xl p-6 w-full max-w-sm shadow-lg">
        <h3 class="text-lg font-semibold text-gray-900 mb-2">{{ $t('global.delete') }}</h3>
        <p class="text-sm text-gray-600 mb-6">{{ moduleToDelete.name }}</p>
        <div class="flex justify-end gap-3">
          <button
            @click="moduleToDelete = null"
            class="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
          >
            {{ $t("global.cancel") }}
          </button>
          <button
            @click="handleDelete"
            class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition"
          >
            {{ $t("global.delete") }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted } from "vue";
import { useNotification } from "@kyvg/vue3-notification";
import { Plus, Search, Pencil, Trash2, ChevronLeft, ChevronRight, BookOpen, Eye, Users } from "lucide-vue-next";
import { useModulesService } from "../../../inversify.config";
import type { ModuleDto } from "../../../types/entities";
import AssignModuleModal from "./AssignModuleModal.vue";

const backendUrl = (import.meta.env.VITE_API_BASE_URL || "").replace(
  /\/api$/,
  "",
);

const { notify } = useNotification();
const modulesService = useModulesService();

const allModules = ref<ModuleDto[]>([]);
const loading = ref(true);
const searchValue = ref("");
const pageIndex = ref(1);
const pageSize = 9;
const moduleToDelete = ref<ModuleDto | null>(null);
const showAssignModal = ref(false);
const selectedModuleForAssignment = ref<ModuleDto | null>(null);

function imageUrl(path: string | undefined): string {
  if (!path) return "";
  if (path.startsWith("http")) return path;
  return backendUrl + path;
}

function plainText(value: string | undefined): string {
  if (!value) return "---";
  return value.replace(/<[^>]*>/g, " ").replace(/\s+/g, " ").trim() || "---";
}

const filtered = computed(() => {
  const q = searchValue.value.toLowerCase().trim();
  if (!q) return allModules.value;
  return allModules.value.filter(m =>
    (m.name || "").toLowerCase().includes(q) ||
    (m.subject || "").toLowerCase().includes(q) ||
    (m.content || "").toLowerCase().includes(q)
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

function openAssignModal(mod: ModuleDto) {
  selectedModuleForAssignment.value = mod;
  showAssignModal.value = true;
}

async function onAssigned() {
  showAssignModal.value = false;
  selectedModuleForAssignment.value = null;
  await fetchModules();
}

async function handleDelete() {
  if (!moduleToDelete.value?.id) return;
  try {
    const result = await modulesService.deleteModule(moduleToDelete.value.id);
    if (result.succeeded) {
      notify({ type: "success", text: "Module deleted." });
      moduleToDelete.value = null;
      await fetchModules();
    } else {
      notify({ type: "error", text: "Error deleting module." });
    }
  } catch {
    notify({ type: "error", text: "Error deleting module." });
  }
}

onMounted(fetchModules);
</script>
