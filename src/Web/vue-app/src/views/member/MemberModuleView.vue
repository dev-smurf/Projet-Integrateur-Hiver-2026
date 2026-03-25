<template>
  <div class="max-w-4xl mx-auto">
    <!-- Loading -->
    <div v-if="loading" class="animate-pulse space-y-4">
      <div class="h-8 bg-gray-200 rounded w-1/2" />
      <div class="h-4 bg-gray-200 rounded w-1/3" />
      <div class="h-48 bg-gray-200 rounded" />
    </div>

    <!-- Error -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4">
      <p class="text-sm text-red-600">{{ error }}</p>
      <router-link :to="{ name: 'member.modules.index' }" class="text-sm text-brand-600 hover:underline mt-2 inline-block">
        Retour aux modules
      </router-link>
    </div>

    <!-- Module content -->
    <div v-else-if="module">
      <!-- Header -->
      <div class="mb-8">
        <router-link :to="{ name: 'member.modules.index' }" class="text-sm text-brand-600 hover:underline mb-4 inline-block">
          &larr; Retour aux modules
        </router-link>
        <h1 class="text-3xl font-bold text-gray-900 mb-2">{{ module.name }}</h1>
        <p v-if="module.subject" class="text-lg text-gray-600">{{ module.subject }}</p>
      </div>

      <!-- Card image -->
      <div v-if="module.cardImageUrl" class="mb-8">
        <img
          :src="imageUrl(module.cardImageUrl)"
          :alt="module.name"
          class="w-full max-h-80 object-cover rounded-xl"
        />
      </div>

      <!-- Module content -->
      <div v-if="module.content" class="module-content prose max-w-none mb-8" v-html="module.content"></div>

      <!-- Sections -->
      <div v-if="module.sections && module.sections.length" class="space-y-8">
        <div
          v-for="section in module.sections"
          :key="section.id"
          class="bg-white rounded-xl border border-gray-200 overflow-hidden"
        >
          <div class="px-6 py-4 bg-gray-50 border-b border-gray-200">
            <h2 class="text-xl font-semibold text-gray-900">{{ section.title }}</h2>
          </div>
          <div v-if="section.content" class="module-content p-6" v-html="section.content"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from "vue";
import { useModulesService } from "@/inversify.config";
import type { ModuleDto } from "@/types/entities";
import '@/components/editor/module-content.css';
import '@/components/editor/module-blocks.css';

const backendUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/api$/, '');
const props = defineProps<{ moduleId: string }>();
const modulesService = useModulesService();

const module = ref<ModuleDto | null>(null);
const loading = ref(true);
const error = ref<string | null>(null);

function imageUrl(path: string | undefined): string {
  if (!path) return '';
  if (path.startsWith('http')) return path;
  return backendUrl + path;
}

onMounted(async () => {
  try {
    module.value = await modulesService.getMyModuleDetail(props.moduleId);
  } catch (e: any) {
    if (e.response?.status === 403) {
      error.value = "Vous n'avez pas accès à ce module.";
    } else {
      error.value = "Impossible de charger le module.";
    }
  }
  loading.value = false;
});
</script>
