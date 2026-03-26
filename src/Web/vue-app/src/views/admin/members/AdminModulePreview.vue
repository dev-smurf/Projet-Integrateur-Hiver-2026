<template>
  <div class="flex gap-6">
    <!-- Main content -->
    <div class="flex-1 min-w-0">
      <!-- Header -->
      <div class="flex items-center gap-3 mb-6">
        <router-link
          :to="{ name: 'admin.children.modules.edit', params: { id: props.id } }"
          class="flex items-center gap-1.5 text-sm text-brand-600 hover:underline"
        >
          <ArrowLeft class="w-4 h-4" />
          Retour à l'édition
        </router-link>
        <span class="text-sm text-gray-400">|</span>
        <span class="inline-flex items-center gap-1.5 text-xs font-medium text-amber-700 bg-amber-50 border border-amber-200 rounded-full px-2.5 py-0.5">
          <Eye class="w-3.5 h-3.5" />
          Aperçu
        </span>
      </div>

      <!-- Loading -->
      <div v-if="loading" class="animate-pulse space-y-4">
        <div class="h-8 bg-gray-200 rounded w-1/2" />
        <div class="h-4 bg-gray-200 rounded w-1/3" />
        <div class="h-48 bg-gray-200 rounded" />
      </div>

      <!-- Error -->
      <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4">
        <p class="text-sm text-red-600">{{ error }}</p>
      </div>

      <!-- Module content -->
      <div v-else-if="mod">
        <h1 class="text-3xl font-bold text-gray-900 mb-2">{{ mod.name }}</h1>
        <p v-if="mod.subject" class="text-lg text-gray-600 mb-6">{{ mod.subject }}</p>

        <div v-if="mod.cardImageUrl" class="mb-8">
          <img
            :src="imageUrl(mod.cardImageUrl)"
            :alt="mod.name"
            class="w-full max-h-80 object-cover rounded-xl"
          />
        </div>

        <div v-if="mod.content" class="module-content prose max-w-none mb-8" v-html="mod.content"></div>

        <!-- Sections -->
        <div v-if="mod.sections && mod.sections.length" class="space-y-8">
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
    </div>

    <!-- Sidebar -->
    <aside
      v-if="mod && mod.sections && mod.sections.length"
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
</template>

<script lang="ts" setup>
import { ref, computed, onMounted, onBeforeUnmount } from "vue";
import { ArrowLeft, Eye } from "lucide-vue-next";
import { useModulesService } from "@/inversify.config";
import type { ModuleDto } from "@/types/entities";
import type { ModuleSectionDto } from "@/types/entities/moduleSection";
import '@/components/editor/module-content.css';
import '@/components/editor/module-blocks.css';

const backendUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/api$/, '');
const props = defineProps<{ id: string }>();
const modulesService = useModulesService();

const mod = ref<ModuleDto | null>(null);
const loading = ref(true);
const error = ref<string | null>(null);
const activeSection = ref<string | null>(null);

let observer: IntersectionObserver | null = null;

const sortedSections = computed(() => {
  if (!mod.value?.sections) return [];
  return [...mod.value.sections].sort((a: ModuleSectionDto, b: ModuleSectionDto) => a.sortOrder - b.sortOrder);
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
  if (!mod.value?.sections?.length) return;

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

  for (const section of mod.value.sections) {
    const el = document.getElementById('section-' + section.id);
    if (el) observer.observe(el);
  }
}

onMounted(async () => {
  try {
    mod.value = await modulesService.getModuleFlexible(props.id);
    if (!mod.value) {
      error.value = "Module introuvable.";
    } else if (mod.value.sections?.length) {
      activeSection.value = sortedSections.value[0]?.id ?? null;
      // Wait for DOM to render sections before observing
      setTimeout(setupObserver, 100);
    }
  } catch {
    error.value = "Impossible de charger le module.";
  }
  loading.value = false;
});

onBeforeUnmount(() => {
  observer?.disconnect();
});
</script>
