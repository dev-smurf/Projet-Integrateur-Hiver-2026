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
        <span class="text-xs text-gray-400 italic">
          {{ $t('modulePages.clickToEdit') }}
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

        <div v-if="mod.cardImageUrl" class="mb-6">
          <img
            :src="imageUrl(mod.cardImageUrl)"
            :alt="mod.name"
            class="w-full max-h-64 object-cover rounded-xl"
          />
        </div>

        <!-- Module intro shown only on first page -->
        <div
          v-if="mod.content && currentPageIndex === 0"
          class="module-content prose max-w-none mb-6"
          v-html="mod.content"
        ></div>

        <!-- Current page (clickable header = edit page title) -->
        <div v-if="currentPage" class="bg-white rounded-xl border border-gray-200 overflow-hidden mb-6">
          <div
            class="px-6 py-4 bg-gray-50 border-b border-gray-200 flex items-center gap-3 cursor-pointer hover:bg-amber-50 transition group"
            @click.stop="goToEdit(currentPage.id, null)"
            :title="$t('modulePages.editPageTitle')"
          >
            <span class="w-8 h-8 shrink-0 rounded-full bg-brand-100 text-brand-700 text-sm font-bold flex items-center justify-center">
              {{ currentPageIndex + 1 }}
            </span>
            <h2 class="text-xl font-semibold text-gray-900 truncate flex-1">{{ currentPage.title }}</h2>
            <Pencil class="w-4 h-4 text-gray-400 opacity-0 group-hover:opacity-100 transition" />
          </div>
          <div v-if="currentPageSections.length > 0" class="divide-y divide-gray-100">
            <div
              v-for="section in currentPageSections"
              :key="section.id"
              class="px-6 py-5 cursor-pointer hover:bg-amber-50/50 transition relative group"
              @click.stop="goToEdit(currentPage.id, section.id)"
              :title="$t('modulePages.editSection')"
            >
              <span class="absolute top-3 right-3 text-gray-400 opacity-0 group-hover:opacity-100 transition">
                <Pencil class="w-4 h-4" />
              </span>
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

        <!-- Prev / indicator / Next -->
        <div v-if="sortedPages.length" class="flex items-center justify-between gap-3">
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
            {{ $t('modulePages.pageOfTotal', { current: currentPageIndex + 1, total: sortedPages.length }) }}
          </span>

          <button
            type="button"
            @click="goToPage(currentPageIndex + 1)"
            :disabled="currentPageIndex >= sortedPages.length - 1"
            class="flex items-center gap-1.5 px-4 py-2 rounded-lg bg-brand-600 text-white text-sm font-medium hover:bg-brand-700 transition disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
          >
            {{ $t('modulePages.next') }}
            <ChevronRight class="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>

    <!-- Sidebar -->
    <aside
      v-if="mod && sortedPages.length"
      class="hidden lg:block w-64 shrink-0"
    >
      <nav class="sticky top-20 bg-white rounded-xl border border-gray-200 p-4">
        <h3 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-3">{{ $t('modulePages.title') }}</h3>
        <ul class="space-y-1">
          <li v-for="(page, idx) in sortedPages" :key="page.id">
            <button
              type="button"
              @click="goToPage(idx)"
              class="w-full flex items-center gap-2 px-3 py-2 text-sm rounded-lg transition text-left cursor-pointer"
              :class="idx === currentPageIndex
                ? 'bg-brand-50 text-brand-700 font-medium'
                : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
            >
              <span class="h-5 w-5 shrink-0 rounded-full border text-[11px] flex items-center justify-center font-semibold"
                :class="idx === currentPageIndex ? 'border-brand-500 text-brand-700' : 'border-gray-300 text-gray-400'"
              >{{ idx + 1 }}</span>
              <span class="truncate">{{ page.title || $t('modulePages.newPage') }}</span>
            </button>
          </li>
        </ul>
      </nav>
    </aside>

  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { ArrowLeft, Eye, ChevronLeft, ChevronRight, Pencil } from "lucide-vue-next";
import { useModulesService } from "@/inversify.config";
import type { ModuleDto } from "@/types/entities";
import type { ModuleSectionDto } from "@/types/entities/moduleSection";
import { parsePageContent, type PageSection } from '@/utils/pageContent';
import '@/components/editor/module-content.css';
import '@/components/editor/module-blocks.css';

const backendUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/api$/, '');
const props = defineProps<{ id: string }>();
const router = useRouter();
const modulesService = useModulesService();

const mod = ref<ModuleDto | null>(null);
const loading = ref(true);
const error = ref<string | null>(null);
const currentPageIndex = ref(0);

const sortedPages = computed(() => {
  if (!mod.value?.sections) return [];
  return [...mod.value.sections].sort((a: ModuleSectionDto, b: ModuleSectionDto) => a.sortOrder - b.sortOrder);
});

const currentPage = computed<ModuleSectionDto | undefined>(() => sortedPages.value[currentPageIndex.value]);

const currentPageSections = computed<PageSection[]>(() =>
  currentPage.value ? parsePageContent(currentPage.value.content ?? '') : []
);

function imageUrl(path: string | undefined): string {
  if (!path) return '';
  if (path.startsWith('http')) return path;
  return backendUrl + path;
}

function goToPage(idx: number) {
  if (idx < 0 || idx >= sortedPages.value.length) return;
  currentPageIndex.value = idx;
  window.scrollTo({ top: 0, behavior: 'smooth' });
}

function goToEdit(pageId: string | null, sectionId: string | null) {
  const query: Record<string, string> = {};
  if (pageId) query.pageId = pageId;
  if (sectionId) query.sectionId = sectionId;
  router.push({ name: 'admin.children.modules.edit', params: { id: props.id }, query });
}

onMounted(async () => {
  try {
    mod.value = await modulesService.getModuleFlexible(props.id);
    if (!mod.value) {
      error.value = "Module introuvable.";
    }
  } catch {
    error.value = "Impossible de charger le module.";
  }
  loading.value = false;
});
</script>
