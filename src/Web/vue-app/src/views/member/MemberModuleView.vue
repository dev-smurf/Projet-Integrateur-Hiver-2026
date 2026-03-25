<template>
  <div>
    <!-- Loading -->
    <div v-if="loading" class="max-w-4xl mx-auto animate-pulse space-y-4">
      <div class="h-8 bg-gray-200 rounded w-1/2" />
      <div class="h-4 bg-gray-200 rounded w-1/3" />
      <div class="h-48 bg-gray-200 rounded" />
    </div>

    <!-- Error -->
    <div v-else-if="error" class="max-w-4xl mx-auto bg-red-50 border border-red-200 rounded-lg p-4">
      <p class="text-sm text-red-600">{{ error }}</p>
      <router-link :to="{ name: 'member.modules.index' }" class="text-sm text-brand-600 hover:underline mt-2 inline-block">
        Retour aux modules
      </router-link>
    </div>

    <!-- Module content with sidebar -->
    <div v-else-if="module" class="flex gap-6">
      <!-- Main content -->
      <div class="flex-1 min-w-0">
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
        <div v-if="sortedSections.length" class="space-y-8">
          <div
            v-for="section in sortedSections"
            :key="section.id"
            :id="'section-' + section.id"
            class="bg-white rounded-xl border border-gray-200 overflow-hidden"
          >
            <div class="px-6 py-4 bg-gray-50 border-b border-gray-200">
              <h2 class="text-xl font-semibold text-gray-900">{{ section.title }}</h2>
            </div>
            <div v-if="section.content" class="module-content p-6" v-html="section.content"></div>
          </div>
        </div>
      </div>

      <!-- Sidebar -->
      <aside
        v-if="sortedSections.length"
        class="hidden lg:block w-64 shrink-0"
      >
        <nav class="sticky top-20 bg-white rounded-xl border border-gray-200 p-4">
          <h3 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-3">Sections</h3>
          <ul class="space-y-1">
            <li v-for="section in sortedSections" :key="section.id">
              <a
                :href="'#section-' + section.id"
                @click.prevent="scrollToSection(section.id)"
                class="block px-3 py-2 text-sm rounded-lg transition"
                :class="activeSection === section.id
                  ? 'bg-brand-50 text-brand-700 font-medium'
                  : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
              >
                {{ section.title }}
              </a>
            </li>
          </ul>
        </nav>
      </aside>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted, onBeforeUnmount } from "vue";
import { useModulesService } from "@/inversify.config";
import type { ModuleDto } from "@/types/entities";
import type { ModuleSectionDto } from "@/types/entities/moduleSection";
import '@/components/editor/module-content.css';
import '@/components/editor/module-blocks.css';

const backendUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/api$/, '');
const props = defineProps<{ moduleId: string }>();
const modulesService = useModulesService();

const module = ref<ModuleDto | null>(null);
const loading = ref(true);
const error = ref<string | null>(null);
const activeSection = ref<string | null>(null);

let observer: IntersectionObserver | null = null;

const sortedSections = computed(() => {
  if (!module.value?.sections) return [];
  return [...module.value.sections].sort((a: ModuleSectionDto, b: ModuleSectionDto) => a.sortOrder - b.sortOrder);
});

function imageUrl(path: string | undefined): string {
  if (!path) return '';
  if (path.startsWith('http')) return path;
  return backendUrl + path;
}

function scrollToSection(id: string) {
  const el = document.getElementById('section-' + id);
  if (el) {
    const top = el.getBoundingClientRect().top + window.scrollY - 72;
    window.scrollTo({ top, behavior: 'smooth' });
  }
}

function setupObserver() {
  if (!sortedSections.value.length) return;

  observer = new IntersectionObserver(
    (entries) => {
      for (const entry of entries) {
        if (entry.isIntersecting) {
          activeSection.value = entry.target.id.replace('section-', '');
        }
      }
    },
    { rootMargin: '-72px 0px -70% 0px' }
  );

  for (const section of sortedSections.value) {
    const el = document.getElementById('section-' + section.id);
    if (el) observer.observe(el);
  }
}

onMounted(async () => {
  try {
    module.value = await modulesService.getMyModuleDetail(props.moduleId);
    if (module.value?.sections?.length) {
      activeSection.value = sortedSections.value[0]?.id ?? null;
      setTimeout(setupObserver, 100);
    }
  } catch (e: any) {
    if (e.response?.status === 403) {
      error.value = "Vous n'avez pas accès à ce module.";
    } else {
      error.value = "Impossible de charger le module.";
    }
  }
  loading.value = false;
});

onBeforeUnmount(() => {
  observer?.disconnect();
});
</script>
