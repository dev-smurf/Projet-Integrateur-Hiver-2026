<template>
  <div>
    <h1 class="text-2xl font-bold text-gray-900 mb-6">Mes modules</h1>

    <!-- Skeleton loading -->
    <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="n in 3" :key="n" class="bg-white rounded-xl border border-gray-200 overflow-hidden animate-pulse">
        <div class="h-44 bg-gray-200" />
        <div class="p-4 space-y-3">
          <div class="h-5 bg-gray-200 rounded w-3/4" />
          <div class="h-3 bg-gray-200 rounded w-full" />
        </div>
      </div>
    </div>

    <!-- Empty state -->
    <div v-else-if="!modules.length" class="text-center py-12 text-gray-500">
      Aucun module assigné.
    </div>

    <!-- Card grid -->
    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
      <router-link
        v-for="mod in modules"
        :key="mod.id"
        :to="'/mes-modules/' + mod.id"
        class="bg-white rounded-xl border border-gray-200 overflow-hidden flex flex-col hover:shadow-md transition-shadow"
      >
        <div class="h-44 bg-gray-50 flex items-center justify-center overflow-hidden">
          <img
            v-if="mod.cardImageUrl"
            :src="imageUrl(mod.cardImageUrl)"
            :alt="mod.name"
            class="w-full h-full object-cover"
          />
          <BookOpen v-else class="w-12 h-12 text-brand-400" />
        </div>
        <div class="p-4 flex flex-col flex-1">
          <h3 class="font-semibold text-gray-900 mb-1 line-clamp-1">{{ mod.name || '—' }}</h3>
          <p class="text-sm text-gray-500 line-clamp-3 flex-1">{{ mod.subject || '—' }}</p>
        </div>
      </router-link>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from "vue";
import { BookOpen } from "lucide-vue-next";
import { useModulesService } from "@/inversify.config";
import type { ModuleDto } from "@/types/entities";

const backendUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/api$/, '');
const modulesService = useModulesService();

const modules = ref<ModuleDto[]>([]);
const loading = ref(true);

function imageUrl(path: string | undefined): string {
  if (!path) return '';
  if (path.startsWith('http')) return path;
  return backendUrl + path;
}

onMounted(async () => {
  try {
    modules.value = await modulesService.getMyModules();
  } catch {
    modules.value = [];
  }
  loading.value = false;
});
</script>
