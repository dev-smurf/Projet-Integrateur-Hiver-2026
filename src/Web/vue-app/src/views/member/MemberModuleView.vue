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
        {{ $t('member.modules.backToList') }}
      </router-link>
    </div>

    <!-- Module content -->
    <div v-else-if="module" class="flex gap-6">
      <!-- Main content -->
      <div class="flex-1 min-w-0">
        <!-- Header -->
        <div class="mb-6">
          <router-link :to="{ name: 'member.modules.index' }" class="text-sm text-brand-600 hover:underline mb-4 inline-block">
            &larr; {{ $t('member.modules.backToList') }}
          </router-link>
          <h1 class="text-3xl font-bold text-gray-900 mb-2">{{ module.name }}</h1>
          <p v-if="module.subject" class="text-lg text-gray-600">{{ module.subject }}</p>
        </div>

        <!-- Card image -->
        <div v-if="module.cardImageUrl" class="mb-6">
          <img
            :src="imageUrl(module.cardImageUrl)"
            :alt="module.name"
            class="w-full max-h-64 object-cover rounded-xl"
          />
        </div>

        <!-- Module-level content (shown only on first page for context) -->
        <div
          v-if="module.content && currentPageIndex === 0"
          class="module-content prose max-w-none mb-6"
          v-html="module.content"
        ></div>

        <!-- Progress bar -->
        <div v-if="sortedSections.length" class="mb-6 bg-white rounded-xl border border-gray-200 p-4">
          <div class="flex items-center justify-between text-sm mb-2">
            <span class="text-gray-600 font-medium">{{ $t('modulePages.progress') }}</span>
            <span class="text-brand-600 font-semibold">{{ progressPercent }}%</span>
          </div>
          <div class="h-2.5 rounded-full bg-gray-100">
            <div
              class="h-full rounded-full bg-brand-500 transition-all duration-500"
              :style="{ width: progressPercent + '%' }"
            />
          </div>
          <p class="text-xs text-gray-400 mt-2">
            {{ $t('modulePages.pagesRead', { read: readCount, total: sortedSections.length }) }}
          </p>
        </div>

        <!-- Current page -->
        <div v-if="currentPage" class="bg-white rounded-xl border border-gray-200 overflow-hidden mb-6">
          <div class="px-6 py-4 bg-gray-50 border-b border-gray-200 flex items-center justify-between">
            <div class="flex items-center gap-3 min-w-0">
              <span class="w-8 h-8 shrink-0 rounded-full bg-brand-100 text-brand-700 text-sm font-bold flex items-center justify-center">
                {{ currentPageIndex + 1 }}
              </span>
              <h2 class="text-xl font-semibold text-gray-900 truncate">{{ currentPage.title }}</h2>
            </div>
            <CheckCircle
              v-if="readSections.has(currentPage.id)"
              class="h-5 w-5 text-emerald-500 shrink-0"
            />
          </div>
          <div v-if="currentPageSections.length > 0" class="divide-y divide-gray-100">
            <div
              v-for="section in currentPageSections"
              :key="section.id"
              class="px-6 py-5"
            >
              <h3 v-if="section.title" class="text-lg font-semibold text-gray-800 mb-3">
                {{ section.title }}
              </h3>
              <div
                v-if="section.content"
                class="module-content prose max-w-none"
                v-html="section.content"
              ></div>
            </div>
          </div>
          <div v-else class="p-6 text-sm text-gray-400 italic">
            {{ $t('modulePages.noContent') }}
          </div>
        </div>

        <!-- Prev / page indicator / Next -->
        <div v-if="sortedSections.length" class="flex items-center justify-between gap-3">
          <button
            type="button"
            @click="goToPage(currentPageIndex - 1)"
            :disabled="currentPageIndex === 0"
            class="flex items-center gap-1.5 px-4 py-2 rounded-lg border border-gray-300 bg-white text-sm font-medium text-gray-700 hover:bg-gray-50 transition disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
          >
            <ChevronLeft class="w-4 h-4" />
            {{ $t('modulePages.previous') }}
          </button>

          <span class="text-sm text-gray-500">
            {{ $t('modulePages.pageOfTotal', { current: currentPageIndex + 1, total: sortedSections.length }) }}
          </span>

          <button
            v-if="isLastPage"
            type="button"
            @click="finishModule"
            class="flex items-center gap-1.5 px-4 py-2 rounded-lg bg-emerald-600 text-white text-sm font-medium hover:bg-emerald-700 transition cursor-pointer"
          >
            <CheckCircle class="w-4 h-4" />
            {{ $t('modulePages.finish') }}
          </button>
          <button
            v-else
            type="button"
            @click="goToPage(currentPageIndex + 1)"
            class="flex items-center gap-1.5 px-4 py-2 rounded-lg bg-brand-600 text-white text-sm font-medium hover:bg-brand-700 transition cursor-pointer"
          >
            {{ $t('modulePages.next') }}
            <ChevronRight class="w-4 h-4" />
          </button>
        </div>
      </div>

      <!-- Desktop Sidebar -->
      <aside
        v-if="sortedSections.length"
        class="hidden lg:block w-64 shrink-0"
      >
        <nav class="sticky top-20 bg-white rounded-xl border border-gray-200 p-4">
          <h3 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-3">{{ $t('modulePages.title') }}</h3>
          <ul class="space-y-1">
            <li v-for="(page, idx) in sortedSections" :key="page.id">
              <button
                type="button"
                @click="goToPage(idx)"
                class="w-full flex items-center gap-2 px-3 py-2 text-sm rounded-lg transition text-left cursor-pointer"
                :class="idx === currentPageIndex
                  ? 'bg-brand-50 text-brand-700 font-medium'
                  : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
              >
                <CheckCircle
                  v-if="readSections.has(page.id)"
                  class="h-4 w-4 text-emerald-500 shrink-0"
                />
                <span v-else class="h-4 w-4 shrink-0 rounded-full border border-gray-300 text-[10px] flex items-center justify-center text-gray-400 font-semibold">
                  {{ idx + 1 }}
                </span>
                <span class="truncate">{{ page.title || $t('modulePages.newPage') }}</span>
              </button>
            </li>
          </ul>
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

      <!-- Mobile FAB -->
      <button
        v-if="sortedSections.length"
        @click="mobileNavOpen = !mobileNavOpen"
        class="lg:hidden fixed bottom-6 right-6 z-40 h-14 w-14 rounded-full bg-brand-600 text-white shadow-lg flex items-center justify-center hover:bg-brand-700 transition"
      >
        <List class="h-6 w-6" />
      </button>

      <!-- Mobile drawer -->
      <Teleport to="body">
        <div v-if="mobileNavOpen" class="lg:hidden fixed inset-0 z-50">
          <div class="absolute inset-0 bg-black/40" @click="mobileNavOpen = false" />
          <div class="absolute bottom-0 left-0 right-0 bg-white rounded-t-2xl max-h-[70vh] overflow-y-auto p-6 shadow-xl">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-gray-900">{{ $t('modulePages.title') }}</h3>
              <button @click="mobileNavOpen = false" class="text-gray-400 hover:text-gray-600">
                <X class="h-5 w-5" />
              </button>
            </div>
            <div class="mb-4">
              <div class="flex items-center justify-between text-sm text-gray-500 mb-1">
                <span>{{ $t('modulePages.pagesRead', { read: readCount, total: sortedSections.length }) }}</span>
                <span class="font-semibold text-brand-600">{{ progressPercent }}%</span>
              </div>
              <div class="h-2 rounded-full bg-gray-100">
                <div class="h-full rounded-full bg-brand-500 transition-all" :style="{ width: progressPercent + '%' }" />
              </div>
            </div>
            <ul class="space-y-1">
              <li v-for="(page, idx) in sortedSections" :key="page.id">
                <button
                  type="button"
                  @click="goToPage(idx); mobileNavOpen = false"
                  class="w-full flex items-center gap-3 px-3 py-3 text-sm rounded-lg transition text-left cursor-pointer"
                  :class="idx === currentPageIndex
                    ? 'bg-brand-50 text-brand-700 font-medium'
                    : 'text-gray-600 hover:bg-gray-50'"
                >
                  <CheckCircle
                    v-if="readSections.has(page.id)"
                    class="h-5 w-5 text-emerald-500 shrink-0"
                  />
                  <span v-else class="h-5 w-5 shrink-0 rounded-full border-2 border-gray-300 text-[10px] flex items-center justify-center text-gray-400 font-semibold">
                    {{ idx + 1 }}
                  </span>
                  <span class="truncate">{{ page.title || $t('modulePages.newPage') }}</span>
                </button>
              </li>
            </ul>
          </div>
        </div>
      </Teleport>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted, onBeforeUnmount, watch } from "vue";
import { useRouter } from "vue-router";
import { useModulesService } from "@/inversify.config";
import type { ModuleDto } from "@/types/entities";
import type { ModuleSectionDto } from "@/types/entities/moduleSection";
import { CheckCircle, List, X, ChevronLeft, ChevronRight } from "lucide-vue-next";
import { parsePageContent, type PageSection } from '@/utils/pageContent';
import '@/components/editor/module-content.css';
import '@/components/editor/module-blocks.css';

const backendUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/api$/, '');
const props = defineProps<{ moduleId: string }>();
const router = useRouter();
const modulesService = useModulesService();

const module = ref<ModuleDto | null>(null);
const loading = ref(true);
const error = ref<string | null>(null);
const currentPageIndex = ref(0);
const mobileNavOpen = ref(false);
const readSections = ref<Set<string>>(new Set());

const sortedSections = computed(() => {
  if (!module.value?.sections) return [];
  return [...module.value.sections].sort((a: ModuleSectionDto, b: ModuleSectionDto) => a.sortOrder - b.sortOrder);
});

const currentPage = computed<ModuleSectionDto | undefined>(() => sortedSections.value[currentPageIndex.value]);

const currentPageSections = computed<PageSection[]>(() =>
  currentPage.value ? parsePageContent(currentPage.value.content ?? '') : []
);

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

const isLastPage = computed(() =>
  sortedSections.value.length > 0 && currentPageIndex.value === sortedSections.value.length - 1
);

function goToPage(idx: number) {
  if (idx < 0 || idx >= sortedSections.value.length) return;
  currentPageIndex.value = idx;
  // Scroll to top of page smoothly so the user sees the new page from its start.
  window.scrollTo({ top: 0, behavior: 'smooth' });
}

async function finishModule() {
  // Ensure the last page is marked as read before leaving.
  const last = currentPage.value;
  if (last && !readSections.value.has(last.id)) {
    await markPageAsRead(last.id);
  }
  router.push({ name: 'member.modules.index' });
}

async function markPageAsRead(pageId: string) {
  if (readSections.value.has(pageId)) return;
  readSections.value = new Set([...readSections.value, pageId]);
  try {
    await modulesService.markSectionRead(props.moduleId, pageId);
  } catch {
    // Non-blocking
  }
}

// Mark the currently-displayed page as read immediately, so the progress
// bar advances as soon as the member switches pages (no 3s wait).
watch(currentPage, (page) => {
  if (page && !readSections.value.has(page.id)) {
    markPageAsRead(page.id);
  }
});

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
      await loadSectionProgress();

      // Resume on the first unread page (or stay on 0 if the member has
      // read everything — let them review from the start).
      const firstUnread = sortedSections.value.findIndex(s => !readSections.value.has(s.id));
      if (firstUnread > 0) {
        currentPageIndex.value = firstUnread;
      }

      // Mark the page the member lands on as read right away so the progress
      // jumps by one immediately.
      const landingPage = currentPage.value;
      if (landingPage && !readSections.value.has(landingPage.id)) {
        markPageAsRead(landingPage.id);
      }
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
  // no-op (no pending timers to clean up)
});
</script>
