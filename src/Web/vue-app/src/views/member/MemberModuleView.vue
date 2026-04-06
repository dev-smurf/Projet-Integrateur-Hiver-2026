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
        <div class="mb-6">
          <router-link :to="{ name: 'member.modules.index' }" class="text-sm text-brand-600 hover:underline mb-4 inline-block">
            &larr; Retour aux modules
          </router-link>
          <h1 class="text-3xl font-bold text-gray-900 mb-2">{{ module.name }}</h1>
          <p v-if="module.subject" class="text-lg text-gray-600">{{ module.subject }}</p>
        </div>

        <!-- Progress bar -->
        <div v-if="sortedSections.length" class="mb-6 bg-white rounded-xl border border-gray-200 p-4">
          <div class="flex items-center justify-between text-sm mb-2">
            <span class="text-gray-600 font-medium">Progression</span>
            <span class="text-brand-600 font-semibold">{{ progressPercent }}%</span>
          </div>
          <div class="h-2.5 rounded-full bg-gray-100">
            <div
              class="h-full rounded-full bg-brand-500 transition-all duration-500"
              :style="{ width: progressPercent + '%' }"
            />
          </div>
          <p class="text-xs text-gray-400 mt-2">{{ readCount }} / {{ sortedSections.length }} sections lues</p>
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
            <div class="px-6 py-4 bg-gray-50 border-b border-gray-200 flex items-center justify-between">
              <h2 class="text-xl font-semibold text-gray-900">{{ section.title }}</h2>
              <CheckCircle
                v-if="readSections.has(section.id)"
                class="h-5 w-5 text-emerald-500 shrink-0"
              />
            </div>
            <div v-if="section.content" class="module-content p-6" v-html="section.content"></div>
          </div>
        </div>
      </div>

      <!-- Desktop Sidebar -->
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
                class="flex items-center gap-2 px-3 py-2 text-sm rounded-lg transition"
                :class="activeSection === section.id
                  ? 'bg-brand-50 text-brand-700 font-medium'
                  : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
              >
                <CheckCircle
                  v-if="readSections.has(section.id)"
                  class="h-4 w-4 text-emerald-500 shrink-0"
                />
                <span class="h-4 w-4 shrink-0 rounded-full border border-gray-300" v-else />
                <span class="truncate">{{ section.title }}</span>
              </a>
            </li>
          </ul>
          <!-- Sidebar progress -->
          <div class="mt-4 pt-4 border-t border-gray-100">
            <div class="flex items-center justify-between text-xs text-gray-500 mb-1">
              <span>{{ readCount }}/{{ sortedSections.length }}</span>
              <span>{{ progressPercent }}%</span>
            </div>
            <div class="h-1.5 rounded-full bg-gray-100">
              <div class="h-full rounded-full bg-brand-500 transition-all duration-500" :style="{ width: progressPercent + '%' }" />
            </div>
          </div>
        </nav>
      </aside>

      <!-- Mobile FAB for table of contents -->
      <button
        v-if="sortedSections.length"
        @click="mobileNavOpen = !mobileNavOpen"
        class="lg:hidden fixed bottom-6 right-6 z-40 h-14 w-14 rounded-full bg-brand-600 text-white shadow-lg flex items-center justify-center hover:bg-brand-700 transition"
      >
        <List class="h-6 w-6" />
      </button>

      <!-- Mobile drawer -->
      <Teleport to="body">
        <div
          v-if="mobileNavOpen"
          class="lg:hidden fixed inset-0 z-50"
        >
          <div class="absolute inset-0 bg-black/40" @click="mobileNavOpen = false" />
          <div class="absolute bottom-0 left-0 right-0 bg-white rounded-t-2xl max-h-[70vh] overflow-y-auto p-6 shadow-xl">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-gray-900">Sections</h3>
              <button @click="mobileNavOpen = false" class="text-gray-400 hover:text-gray-600">
                <X class="h-5 w-5" />
              </button>
            </div>
            <div class="mb-4">
              <div class="flex items-center justify-between text-sm text-gray-500 mb-1">
                <span>{{ readCount }}/{{ sortedSections.length }} sections</span>
                <span class="font-semibold text-brand-600">{{ progressPercent }}%</span>
              </div>
              <div class="h-2 rounded-full bg-gray-100">
                <div class="h-full rounded-full bg-brand-500 transition-all" :style="{ width: progressPercent + '%' }" />
              </div>
            </div>
            <ul class="space-y-1">
              <li v-for="section in sortedSections" :key="section.id">
                <a
                  :href="'#section-' + section.id"
                  @click.prevent="scrollToSection(section.id); mobileNavOpen = false"
                  class="flex items-center gap-3 px-3 py-3 text-sm rounded-lg transition"
                  :class="activeSection === section.id
                    ? 'bg-brand-50 text-brand-700 font-medium'
                    : 'text-gray-600 hover:bg-gray-50'"
                >
                  <CheckCircle
                    v-if="readSections.has(section.id)"
                    class="h-5 w-5 text-emerald-500 shrink-0"
                  />
                  <span class="h-5 w-5 shrink-0 rounded-full border-2 border-gray-300" v-else />
                  <span>{{ section.title }}</span>
                </a>
              </li>
            </ul>
          </div>
        </div>
      </Teleport>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted, onBeforeUnmount } from "vue";
import { useModulesService } from "@/inversify.config";
import type { ModuleDto } from "@/types/entities";
import type { ModuleSectionDto } from "@/types/entities/moduleSection";
import { CheckCircle, List, X } from "lucide-vue-next";
import '@/components/editor/module-content.css';
import '@/components/editor/module-blocks.css';

const backendUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/api$/, '');
const props = defineProps<{ moduleId: string }>();
const modulesService = useModulesService();

const module = ref<ModuleDto | null>(null);
const loading = ref(true);
const error = ref<string | null>(null);
const activeSection = ref<string | null>(null);
const mobileNavOpen = ref(false);
const readSections = ref<Set<string>>(new Set());

let observer: IntersectionObserver | null = null;
const sectionTimers: Record<string, number> = {};
const SCROLL_READ_DELAY_MS = 3000;

const sortedSections = computed(() => {
  if (!module.value?.sections) return [];
  return [...module.value.sections].sort((a: ModuleSectionDto, b: ModuleSectionDto) => a.sortOrder - b.sortOrder);
});

const readCount = computed(() => readSections.value.size);

const progressPercent = computed(() => {
  if (!sortedSections.value.length) return 0;
  return Math.round((readSections.value.size / sortedSections.value.length) * 100);
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

async function markSectionAsRead(sectionId: string) {
  if (readSections.value.has(sectionId)) return;
  readSections.value = new Set([...readSections.value, sectionId]);
  try {
    await modulesService.markSectionRead(props.moduleId, sectionId);
  } catch {
    // Non-blocking — local state already updated
  }
}

function setupObserver() {
  if (!sortedSections.value.length) return;

  observer = new IntersectionObserver(
    (entries) => {
      for (const entry of entries) {
        const sectionId = entry.target.id.replace('section-', '');

        if (entry.isIntersecting) {
          activeSection.value = sectionId;

          // Start timer to mark as read after 3 seconds
          if (!readSections.value.has(sectionId) && !sectionTimers[sectionId]) {
            sectionTimers[sectionId] = window.setTimeout(() => {
              markSectionAsRead(sectionId);
              delete sectionTimers[sectionId];
            }, SCROLL_READ_DELAY_MS);
          }
        } else {
          // Cancel timer if user scrolls away before 3 seconds
          if (sectionTimers[sectionId]) {
            window.clearTimeout(sectionTimers[sectionId]);
            delete sectionTimers[sectionId];
          }
        }
      }
    },
    { rootMargin: '-72px 0px -30% 0px', threshold: 0.1 }
  );

  for (const section of sortedSections.value) {
    const el = document.getElementById('section-' + section.id);
    if (el) observer.observe(el);
  }
}

async function loadSectionProgress() {
  try {
    const progress = await modulesService.getSectionProgress(props.moduleId);
    readSections.value = new Set(
      progress.filter(p => p.isRead).map(p => p.sectionId)
    );
  } catch {
    // Ignore — start fresh
  }
}

onMounted(async () => {
  try {
    module.value = await modulesService.getMyModuleDetail(props.moduleId);
    if (module.value?.sections?.length) {
      activeSection.value = sortedSections.value[0]?.id ?? null;
      await loadSectionProgress();
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
  for (const timer of Object.values(sectionTimers)) {
    window.clearTimeout(timer);
  }
});
</script>
