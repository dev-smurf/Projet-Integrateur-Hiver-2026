<template>
  <div>
    <div class="flex items-center justify-between mb-3">
      <h2 class="text-lg font-semibold text-gray-900">{{ $t('modulePages.title') }}</h2>
      <span v-if="visiblePages.length > 0" class="text-sm text-gray-500">
        {{ $t('modulePages.pageOfTotal', { current: currentPageIndex + 1, total: visiblePages.length }) }}
      </span>
    </div>

    <!-- Page pills strip (click to jump, drag to reorder) -->
    <div class="flex items-center gap-2 mb-4 flex-wrap">
      <VueDraggable
        v-if="visiblePages.length > 0"
        v-model="visiblePages"
        :animation="200"
        @end="onPageReorder"
        class="flex items-center gap-2 flex-wrap"
      >
        <button
          v-for="(page, idx) in visiblePages"
          :key="page._key"
          type="button"
          @click="goToPage(idx)"
          :title="page.title || $t('modulePages.newPage')"
          class="flex items-center gap-2 px-3 py-1.5 rounded-full border text-sm font-medium transition cursor-pointer select-none"
          :class="idx === currentPageIndex
            ? 'bg-brand-600 text-white border-brand-600 shadow'
            : 'bg-white text-gray-700 border-gray-300 hover:border-brand-400 hover:text-brand-600'"
        >
          <span class="w-5 h-5 flex items-center justify-center rounded-full text-xs font-bold"
            :class="idx === currentPageIndex ? 'bg-white/20' : 'bg-gray-100 text-gray-600'"
          >
            {{ idx + 1 }}
          </span>
          <span class="max-w-[120px] truncate">{{ page.title || $t('modulePages.newPage') }}</span>
        </button>
      </VueDraggable>

      <button
        type="button"
        @click="addPage"
        class="flex items-center gap-1 px-3 py-1.5 rounded-full border border-dashed border-brand-400 text-brand-600 text-sm font-medium hover:bg-brand-50 transition cursor-pointer"
      >
        <Plus class="w-4 h-4" />
        {{ $t('modulePages.addPage') }}
      </button>
    </div>

    <!-- Empty state -->
    <div v-if="visiblePages.length === 0"
      class="bg-white border-2 border-dashed border-gray-200 rounded-xl p-10 text-center"
    >
      <FileText class="w-10 h-10 text-gray-300 mx-auto mb-3" />
      <p class="text-sm text-gray-500 mb-4">{{ $t('modulePages.noPages') }}</p>
      <button
        type="button"
        @click="addPage"
        class="inline-flex items-center gap-1.5 px-4 py-2 rounded-lg bg-brand-600 text-white text-sm font-medium hover:bg-brand-700 transition"
      >
        <Plus class="w-4 h-4" />
        {{ $t('modulePages.createFirstPage') }}
      </button>
    </div>

    <!-- Current page editor -->
    <div v-else-if="currentPage" class="bg-white border border-gray-200 rounded-xl overflow-hidden">
      <!-- Page header -->
      <div class="flex items-center gap-2 px-4 py-3 bg-gray-50 border-b border-gray-200">
        <input
          v-model="currentPage.title"
          type="text"
          :placeholder="$t('modulePages.pageTitle')"
          class="flex-1 px-3 py-1.5 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none"
          @input="emitSections"
        />

        <button
          type="button"
          @click="goToPage(currentPageIndex - 1)"
          :disabled="currentPageIndex === 0"
          :title="$t('modulePages.previous')"
          class="p-1.5 rounded text-gray-500 hover:text-brand-600 hover:bg-brand-50 transition disabled:opacity-30 disabled:cursor-not-allowed cursor-pointer"
        >
          <ChevronLeft class="w-5 h-5" />
        </button>
        <button
          type="button"
          @click="goToPage(currentPageIndex + 1)"
          :disabled="currentPageIndex >= visiblePages.length - 1"
          :title="$t('modulePages.next')"
          class="p-1.5 rounded text-gray-500 hover:text-brand-600 hover:bg-brand-50 transition disabled:opacity-30 disabled:cursor-not-allowed cursor-pointer"
        >
          <ChevronRight class="w-5 h-5" />
        </button>
        <button
          type="button"
          @click="deleteCurrentPage"
          :title="$t('modulePages.deletePage')"
          class="p-1.5 rounded text-gray-400 hover:text-red-500 hover:bg-red-50 transition cursor-pointer"
        >
          <Trash2 class="w-5 h-5" />
        </button>
      </div>

      <!-- Sections within the current page -->
      <div class="p-4 space-y-4">
        <VueDraggable
          v-if="pageSections.length > 0"
          v-model="pageSections"
          handle=".section-drag-handle"
          :animation="200"
          @end="commitPageSections"
          class="space-y-4"
        >
          <div
            v-for="(section, idx) in pageSections"
            :key="section.id"
            :ref="(el) => { if (el) sectionRefs[section.id] = el as HTMLElement; }"
            class="border border-gray-200 rounded-lg overflow-hidden bg-white transition"
            :class="highlightedSectionId === section.id ? 'ring-2 ring-amber-400 border-amber-400' : ''"
          >
            <div class="flex items-center gap-2 px-3 py-2 bg-gray-50 border-b border-gray-200">
              <span class="section-drag-handle cursor-grab text-gray-400 hover:text-gray-600 shrink-0" :title="$t('modulePages.dragSection')">
                <GripVertical class="w-4 h-4" />
              </span>

              <!-- Position picker -->
              <div class="relative shrink-0">
                <button
                  type="button"
                  @click.stop="togglePositionMenu(section.id)"
                  :title="$t('modulePages.moveToPosition')"
                  class="flex items-center gap-0.5 px-1.5 py-0.5 rounded border border-gray-300 bg-white hover:border-brand-400 hover:bg-brand-50 transition cursor-pointer text-xs font-semibold text-gray-600"
                >
                  {{ idx + 1 }}
                  <ChevronDown class="w-3 h-3" />
                </button>
                <div
                  v-if="openPositionMenuFor === section.id"
                  class="absolute left-0 top-full mt-1 z-20 bg-white border border-gray-200 rounded-lg shadow-lg py-1 min-w-[140px]"
                >
                  <div class="px-3 py-1 text-[10px] uppercase tracking-wider text-gray-400 font-semibold border-b border-gray-100">
                    {{ $t('modulePages.moveToPosition') }}
                  </div>
                  <button
                    v-for="n in pageSections.length"
                    :key="n"
                    type="button"
                    @click.stop="moveSectionTo(idx, n - 1)"
                    class="w-full text-left px-3 py-1.5 text-sm hover:bg-brand-50 transition cursor-pointer flex items-center justify-between"
                    :class="n - 1 === idx ? 'text-brand-600 font-semibold bg-brand-50/60' : 'text-gray-700'"
                  >
                    <span>{{ n }}</span>
                    <span v-if="n - 1 === idx" class="text-[10px] text-brand-500">← {{ $t('modulePages.currentPosition') }}</span>
                  </button>
                </div>
              </div>

              <input
                v-model="section.title"
                type="text"
                :placeholder="$t('modulePages.sectionTitle')"
                class="flex-1 px-2 py-1 border border-gray-300 rounded text-sm focus:ring-1 focus:ring-brand-500 focus:border-brand-500 outline-none"
                @input="commitPageSections"
              />
              <button
                type="button"
                @click="deleteSection(idx)"
                :title="$t('modulePages.deleteSection')"
                class="p-1 rounded text-gray-400 hover:text-red-500 hover:bg-red-50 transition cursor-pointer"
              >
                <X class="w-4 h-4" />
              </button>
            </div>
            <div class="p-3">
              <RichTextEditor
                :key="section.id"
                :modelValue="section.content || ''"
                @update:modelValue="val => onSectionContentChange(idx, val)"
              />
            </div>
          </div>
        </VueDraggable>

        <div v-else class="text-center py-8 border-2 border-dashed border-gray-200 rounded-lg text-sm text-gray-500">
          {{ $t('modulePages.noSections') }}
        </div>

        <button
          type="button"
          @click="addSection"
          class="w-full flex items-center justify-center gap-1.5 px-3 py-2 rounded-lg border border-dashed border-brand-400 text-brand-600 text-sm font-medium hover:bg-brand-50 transition cursor-pointer"
        >
          <Plus class="w-4 h-4" />
          {{ $t('modulePages.addSection') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, watch, nextTick, onMounted, onBeforeUnmount } from 'vue';
import { VueDraggable } from 'vue-draggable-plus';
import { ChevronLeft, ChevronRight, ChevronDown, Trash2, Plus, FileText, X, GripVertical } from 'lucide-vue-next';
import { useI18n } from 'vue3-i18n';
import RichTextEditor from '@/components/editor/RichTextEditor.vue';
import type { ISectionPayload } from '@/types/requests/ISaveModuleFullRequest';
import { parsePageContent, serializePageContent, newSectionId, type PageSection } from '@/utils/pageContent';

interface LocalPage extends ISectionPayload {
  _key: string;
}

const props = defineProps<{
  sections: ISectionPayload[];
  initialPageId?: string | null;
  initialSectionId?: string | null;
}>();

const emit = defineEmits<{
  'update:sections': [sections: ISectionPayload[]];
}>();

const { t } = useI18n();

let keyCounter = 0;
function makeKey(): string {
  return `page-${Date.now()}-${keyCounter++}`;
}

// All pages (backend "ModuleSection" entities), including soft-deleted ones.
const allPages = ref<LocalPage[]>([]);
const currentPageIndex = ref(0);

// Parsed sub-sections for the currently-selected page. Edited in-memory;
// serialized back into `currentPage.content` whenever it changes.
const pageSections = ref<PageSection[]>([]);

const visiblePages = computed<LocalPage[]>({
  get() {
    return allPages.value.filter(p => !p.isDeleted);
  },
  set(newOrder) {
    const deleted = allPages.value.filter(p => p.isDeleted);
    allPages.value = [...newOrder, ...deleted];
  }
});

const currentPage = computed<LocalPage | undefined>(() => visiblePages.value[currentPageIndex.value]);

// Tracking section card DOM nodes so we can scroll to a specific one when
// the parent passes initialSectionId via a deep link.
const sectionRefs: Record<string, HTMLElement> = {};
const highlightedSectionId = ref<string | null>(null);

// Position picker: which section's position menu is currently open.
const openPositionMenuFor = ref<string | null>(null);
function togglePositionMenu(id: string) {
  openPositionMenuFor.value = openPositionMenuFor.value === id ? null : id;
}
function moveSectionTo(fromIdx: number, toIdx: number) {
  if (fromIdx === toIdx || toIdx < 0 || toIdx >= pageSections.value.length) {
    openPositionMenuFor.value = null;
    return;
  }
  const [moved] = pageSections.value.splice(fromIdx, 1);
  pageSections.value.splice(toIdx, 0, moved);
  openPositionMenuFor.value = null;
  commitPageSections();
}
function onDocClickClosePositionMenu() {
  openPositionMenuFor.value = null;
}

watch(() => props.sections, (newSections) => {
  if (allPages.value.length === 0 && newSections.length > 0) {
    allPages.value = newSections.map(s => ({ ...s, _key: s.id || makeKey() }));

    // Honor deep-link targets from the preview's quick-edit popup.
    if (props.initialPageId) {
      const idx = visiblePages.value.findIndex(p => p.id === props.initialPageId);
      if (idx >= 0) currentPageIndex.value = idx;
    }
  }
}, { immediate: true });

// Reload pageSections whenever the active page changes.
watch(currentPage, (page) => {
  pageSections.value = page ? parsePageContent(page.content ?? '') : [];

  // When the user navigated here via the preview's quick-edit popup with
  // a target sectionId, scroll to that section and flash-highlight it.
  if (props.initialSectionId && page && page.id === props.initialPageId) {
    const targetId = props.initialSectionId;
    nextTick(() => {
      const el = sectionRefs[targetId];
      if (!el) return;
      el.scrollIntoView({ behavior: 'smooth', block: 'center' });
      highlightedSectionId.value = targetId;
      setTimeout(() => {
        if (highlightedSectionId.value === targetId) highlightedSectionId.value = null;
      }, 2500);
    });
  }
}, { immediate: true });

watch(() => visiblePages.value.length, (len) => {
  if (len === 0) {
    currentPageIndex.value = 0;
  } else if (currentPageIndex.value >= len) {
    currentPageIndex.value = len - 1;
  }
});

function goToPage(idx: number) {
  if (idx < 0 || idx >= visiblePages.value.length) return;
  currentPageIndex.value = idx;
}

function addPage() {
  const newPage: LocalPage = {
    _key: makeKey(),
    title: '',
    content: '',
    sortOrder: allPages.value.length,
    isDeleted: false,
  };
  allPages.value.push(newPage);
  currentPageIndex.value = visiblePages.value.length - 1;
  emitSections();
}

function deleteCurrentPage() {
  const page = currentPage.value;
  if (!page) return;
  if (!confirm(t('modulePages.confirmDelete'))) return;

  const fullIndex = allPages.value.findIndex(p => p._key === page._key);
  if (fullIndex === -1) return;

  if (page.id) {
    allPages.value[fullIndex].isDeleted = true;
  } else {
    allPages.value.splice(fullIndex, 1);
  }

  if (currentPageIndex.value > 0 && currentPageIndex.value >= visiblePages.value.length) {
    currentPageIndex.value = Math.max(0, visiblePages.value.length - 1);
  }
  emitSections();
}

function onPageReorder() {
  visiblePages.value.forEach((p, i) => { p.sortOrder = i; });
  emitSections();
}

// -------- sub-section management within the current page --------

function addSection() {
  pageSections.value.push({
    id: newSectionId(),
    title: '',
    content: '',
  });
  commitPageSections();
}

function deleteSection(idx: number) {
  pageSections.value.splice(idx, 1);
  commitPageSections();
}

function onSectionContentChange(idx: number, value: string) {
  if (!pageSections.value[idx]) return;
  pageSections.value[idx].content = value;
  commitPageSections();
}

function commitPageSections() {
  if (!currentPage.value) return;
  currentPage.value.content = serializePageContent(pageSections.value);
  emitSections();
}

function emitSections() {
  const payload: ISectionPayload[] = allPages.value.map(({ _key, ...rest }) => rest);
  emit('update:sections', payload);
}

onMounted(() => {
  document.addEventListener('click', onDocClickClosePositionMenu);
});

onBeforeUnmount(() => {
  document.removeEventListener('click', onDocClickClosePositionMenu);
});
</script>
